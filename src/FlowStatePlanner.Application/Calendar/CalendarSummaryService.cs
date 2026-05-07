using FlowStatePlanner.Application.Abstractions;
using FlowStatePlanner.Application.Calendar.Dtos;
using FlowStatePlanner.Application.DailyPlans;
using FlowStatePlanner.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowStatePlanner.Application.Calendar;

public sealed class CalendarSummaryService(IApplicationDbContext dbContext, ICurrentUserService currentUserService, IRecurrenceRuleMatcher recurrenceMatcher) : ICalendarSummaryService
{
    private const int MaxPreviewItems = 5;

    public async Task<WeekSummaryResponse> GetWeekSummaryAsync(DateOnly date, CancellationToken cancellationToken = default)
    {
        var weekStart = date.AddDays(-((7 + (int)date.DayOfWeek - (int)DayOfWeek.Monday) % 7));
        var weekEnd = weekStart.AddDays(6);
        var days = await BuildDaySummaries(weekStart, weekEnd, null, cancellationToken);
        return new WeekSummaryResponse { WeekStartDate = weekStart, WeekEndDate = weekEnd, Days = days };
    }

    public async Task<MonthSummaryResponse> GetMonthSummaryAsync(int year, int month, CancellationToken cancellationToken = default)
    {
        var monthStart = new DateOnly(year, month, 1);
        var monthEnd = monthStart.AddMonths(1).AddDays(-1);
        var gridStart = monthStart.AddDays(-((7 + (int)monthStart.DayOfWeek - (int)DayOfWeek.Monday) % 7));
        var gridEnd = monthEnd.AddDays((7 - (int)monthEnd.DayOfWeek + (int)DayOfWeek.Sunday) % 7);

        var days = await BuildDaySummaries(gridStart, gridEnd, month, cancellationToken);
        return new MonthSummaryResponse
        {
            Year = year,
            Month = month,
            MonthStartDate = monthStart,
            MonthEndDate = monthEnd,
            CalendarGridStartDate = gridStart,
            CalendarGridEndDate = gridEnd,
            Days = days
        };
    }

    private async Task<IReadOnlyList<CalendarDaySummaryResponse>> BuildDaySummaries(DateOnly start, DateOnly end, int? currentMonth, CancellationToken cancellationToken)
    {
        var userId = currentUserService.UserId;

        var plans = await dbContext.DailyPlans
            .Where(x => x.UserId == userId && x.PlanDate >= start && x.PlanDate <= end)
            .Select(x => new { x.Id, x.PlanDate, Items = x.Items.Where(i => !i.IsDeleted).ToList() })
            .ToListAsync(cancellationToken);

        var oneOffTasks = await dbContext.TaskItems
            .Where(x => x.UserId == userId && !x.IsDeleted && x.TaskType == TaskType.OneOff && x.DueDate >= start && x.DueDate <= end && x.Status != FlowStatePlanner.Domain.Entities.TaskStatus.Done && x.Status != FlowStatePlanner.Domain.Entities.TaskStatus.Archived)
            .ToListAsync(cancellationToken);

        var recurringTasks = await dbContext.TaskItems
            .Where(x => x.UserId == userId && !x.IsDeleted && x.TaskType == TaskType.Recurring)
            .ToListAsync(cancellationToken);

        var templates = await dbContext.RoutineTemplates
            .Where(x => x.UserId == userId && !x.IsDeleted && x.IsActive)
            .Select(x => new { x.AppliesToDays, Blocks = x.Blocks.Where(b => !b.IsDeleted).ToList() })
            .ToListAsync(cancellationToken);

        var planLookup = plans.ToDictionary(x => x.PlanDate);
        var summaries = new List<CalendarDaySummaryResponse>();

        for (var d = start; d <= end; d = d.AddDays(1))
        {
            if (planLookup.TryGetValue(d, out var plan))
            {
                var ordered = plan.Items.OrderBy(i => i.PlannedStartTime.HasValue ? 0 : 1).ThenBy(i => i.PlannedStartTime).ThenBy(i => i.SortOrder).ToList();
                summaries.Add(new CalendarDaySummaryResponse
                {
                    Date = d,
                    DayOfWeek = d.DayOfWeek,
                    IsInCurrentMonth = !currentMonth.HasValue || d.Month == currentMonth,
                    HasGeneratedDailyPlan = true,
                    DailyPlanId = plan.Id,
                    TotalPlannedItemCount = ordered.Count,
                    CompletedItemCount = ordered.Count(i => i.IsCompleted),
                    IncompleteItemCount = ordered.Count(i => !i.IsCompleted),
                    DueOneOffTaskCount = ordered.Count(i => i.SourceType == DailyPlanItemSourceType.TaskItem),
                    RecurringTaskCount = 0,
                    RoutineBlockCount = ordered.Count(i => i.SourceType == DailyPlanItemSourceType.RoutineBlock),
                    HasActiveRoutine = ordered.Any(i => i.SourceType == DailyPlanItemSourceType.RoutineBlock),
                    PreviewItems = ordered.Take(MaxPreviewItems).Select(i => new CalendarPreviewItemResponse { Title = i.Title, SourceType = i.SourceType.ToString(), StartTime = i.PlannedStartTime, IsCompleted = i.IsCompleted }).ToList()
                });
                continue;
            }

            var dayDue = oneOffTasks.Where(t => t.DueDate == d).OrderBy(t => t.StartTime).ToList();
            var dayRecurring = recurringTasks.Where(t => SafeMatches(t.RecurrenceRule, d)).OrderBy(t => t.StartTime).ToList();
            var dayBlocks = templates.Where(t => t.AppliesToDays.Contains(d.DayOfWeek)).SelectMany(t => t.Blocks).OrderBy(b => b.StartTime).ThenBy(b => b.SortOrder).ToList();

            var preview = new List<CalendarPreviewItemResponse>();
            preview.AddRange(dayDue.Select(t => new CalendarPreviewItemResponse { Title = t.Title, SourceType = "TaskItem", StartTime = t.StartTime }));
            preview.AddRange(dayRecurring.Select(t => new CalendarPreviewItemResponse { Title = t.Title, SourceType = "RecurringTask", StartTime = t.StartTime }));
            preview.AddRange(dayBlocks.Select(b => new CalendarPreviewItemResponse { Title = b.Title, SourceType = "RoutineBlock", StartTime = b.StartTime }));

            summaries.Add(new CalendarDaySummaryResponse
            {
                Date = d,
                DayOfWeek = d.DayOfWeek,
                IsInCurrentMonth = !currentMonth.HasValue || d.Month == currentMonth,
                HasGeneratedDailyPlan = false,
                TotalPlannedItemCount = dayDue.Count() + dayRecurring.Count() + dayBlocks.Count(),
                CompletedItemCount = 0,
                IncompleteItemCount = dayDue.Count() + dayRecurring.Count() + dayBlocks.Count(),
                DueOneOffTaskCount = dayDue.Count(),
                RecurringTaskCount = dayRecurring.Count(),
                RoutineBlockCount = dayBlocks.Count(),
                HasActiveRoutine = dayBlocks.Any(),
                PreviewItems = preview.Take(MaxPreviewItems).ToList()
            });
        }

        return summaries;
    }

    private bool SafeMatches(string? recurrenceRule, DateOnly date)
    {
        try { return recurrenceMatcher.Matches(recurrenceRule, date); }
        catch { return false; }
    }
}
