using FlowStatePlanner.Application.Abstractions;
using FlowStatePlanner.Application.DailyPlans.Dtos;
using FlowStatePlanner.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowStatePlanner.Application.DailyPlans;

public sealed class DailyPlanService(IApplicationDbContext dbContext, ICurrentUserService currentUserService, IRecurrenceRuleMatcher matcher) : IDailyPlanService
{
    public async Task<DailyPlanResponse?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var plan = await GetPlanEntity(date, cancellationToken);
        return plan is null ? null : Map(plan);
    }

    public async Task<DailyPlanResponse> GenerateAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var existing = await GetPlanEntity(date, cancellationToken);
        if (existing is not null) return Map(existing);

        var now = DateTimeOffset.UtcNow;
        var plan = new DailyPlan { UserId = currentUserService.UserId, PlanDate = date, GenerationSource = PlanGenerationSource.Hybrid, CreatedAtUtc = now };
        var items = new List<DailyPlanItem>();
        var routineBlocks = await dbContext.RoutineTemplates.Where(t => t.UserId == currentUserService.UserId && t.IsActive && !t.IsDeleted && t.AppliesToDays.Contains(date.DayOfWeek)).SelectMany(t => t.Blocks.Where(b => !b.IsDeleted)).OrderBy(b => b.StartTime.HasValue ? 0 : 1).ThenBy(b => b.StartTime).ThenBy(b => b.SortOrder).ToListAsync(cancellationToken);
        items.AddRange(routineBlocks.Select(b => new DailyPlanItem { DailyPlanId = plan.Id, SourceType = DailyPlanItemSourceType.RoutineBlock, RoutineBlockId = b.Id, Title = b.Title, Description = b.Description, PlannedStartTime = b.StartTime, PlannedEndTime = b.EndTime, SortOrder = b.SortOrder, CreatedAtUtc = now, UpdatedAtUtc = now }));

        var taskItems = await dbContext.TaskItems.Where(t => t.UserId == currentUserService.UserId && !t.IsDeleted && t.Status != Domain.Entities.TaskStatus.Done && t.Status != Domain.Entities.TaskStatus.Archived).ToListAsync(cancellationToken);
        var due = taskItems.Where(t => t.TaskType == TaskType.OneOff && t.DueDate == date);
        var recurring = taskItems.Where(t => t.TaskType == TaskType.Recurring && matcher.Matches(t.RecurrenceRule, date));
        items.AddRange(due.Concat(recurring).Select(t => new DailyPlanItem { DailyPlanId = plan.Id, SourceType = DailyPlanItemSourceType.TaskItem, TaskItemId = t.Id, Title = t.Title, Description = t.Description, PlannedStartTime = t.StartTime, SortOrder = 1000, CreatedAtUtc = now, UpdatedAtUtc = now }));

        var prevDate = date.AddDays(-1);
        var carryForwardTaskIds = await dbContext.DailyPlans.Where(p => p.UserId == currentUserService.UserId && p.PlanDate == prevDate).SelectMany(p => p.Items.Where(i => !i.IsDeleted && !i.IsCompleted && i.SourceType == DailyPlanItemSourceType.TaskItem && i.TaskItemId != null).Select(i => i.TaskItemId!.Value)).ToListAsync(cancellationToken);
        var existingTaskIds = items.Where(i => i.TaskItemId.HasValue).Select(i => i.TaskItemId!.Value).ToHashSet();
        var carryTasks = taskItems.Where(t => carryForwardTaskIds.Contains(t.Id) && t.TaskType == TaskType.OneOff && !existingTaskIds.Contains(t.Id));
        items.AddRange(carryTasks.Select(t => new DailyPlanItem { DailyPlanId = plan.Id, SourceType = DailyPlanItemSourceType.CarryForward, TaskItemId = t.Id, Title = t.Title, Description = t.Description, PlannedStartTime = t.StartTime, SortOrder = 2000, CreatedAtUtc = now, UpdatedAtUtc = now }));

        plan.Items = items;
        dbContext.DailyPlans.Add(plan);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(plan);
    }

    public async Task<DailyPlanItemResponse?> UpdateCompletionAsync(DateOnly date, Guid itemId, bool isCompleted, CancellationToken cancellationToken = default)
    {
        var item = await dbContext.DailyPlanItems.Include(i => i.TaskItem).Include(i => i.DailyPlan).FirstOrDefaultAsync(i => i.Id == itemId && i.DailyPlan.UserId == currentUserService.UserId && i.DailyPlan.PlanDate == date && !i.IsDeleted, cancellationToken);
        if (item is null) return null;
        item.IsCompleted = isCompleted;
        item.UpdatedAtUtc = DateTimeOffset.UtcNow;
        if (isCompleted && item.TaskItem is not null && item.TaskItem.TaskType == TaskType.OneOff)
            item.TaskItem.Status = Domain.Entities.TaskStatus.Done;
        await dbContext.SaveChangesAsync(cancellationToken);
        return new DailyPlanItemResponse { Id = item.Id, SourceType = item.SourceType, RoutineBlockId = item.RoutineBlockId, TaskItemId = item.TaskItemId, Title = item.Title, Description = item.Description, PlannedStartTime = item.PlannedStartTime, PlannedEndTime = item.PlannedEndTime, SortOrder = item.SortOrder, IsCompleted = item.IsCompleted };
    }

    private async Task<DailyPlan?> GetPlanEntity(DateOnly date, CancellationToken cancellationToken) =>
        await dbContext.DailyPlans.Include(x => x.Items.Where(i => !i.IsDeleted)).FirstOrDefaultAsync(x => x.UserId == currentUserService.UserId && x.PlanDate == date, cancellationToken);

    private static DailyPlanResponse Map(DailyPlan p) => new() { Id = p.Id, PlanDate = p.PlanDate, Items = p.Items.Where(i => !i.IsDeleted).OrderBy(i => i.PlannedStartTime.HasValue ? 0 : 1).ThenBy(i => i.PlannedStartTime).ThenBy(i => i.SortOrder).Select(i => new DailyPlanItemResponse { Id = i.Id, SourceType = i.SourceType, RoutineBlockId = i.RoutineBlockId, TaskItemId = i.TaskItemId, Title = i.Title, Description = i.Description, PlannedStartTime = i.PlannedStartTime, PlannedEndTime = i.PlannedEndTime, SortOrder = i.SortOrder, IsCompleted = i.IsCompleted }).ToList() };
}
