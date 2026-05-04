using FlowStatePlanner.Application.Abstractions;
using FlowStatePlanner.Application.Calendar;
using FlowStatePlanner.Application.DailyPlans;
using FlowStatePlanner.Domain.Entities;
using FlowStatePlanner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FlowStatePlanner.Tests;

public class CalendarSummaryServiceTests
{
    [Fact]
    public async Task WeekSummary_Calculates_Monday_Sunday()
    {
        var (service, _) = Create();
        var result = await service.GetWeekSummaryAsync(new DateOnly(2026, 5, 6));
        Assert.Equal(new DateOnly(2026, 5, 4), result.WeekStartDate);
        Assert.Equal(new DateOnly(2026, 5, 10), result.WeekEndDate);
        Assert.Equal(7, result.Days.Count);
    }

    [Fact]
    public async Task MonthSummary_Returns_Grid_For_Requested_Month()
    {
        var (service, _) = Create();
        var result = await service.GetMonthSummaryAsync(2026, 5);
        Assert.Equal(2026, result.Year);
        Assert.Equal(5, result.Month);
        Assert.Contains(result.Days, d => d.Date == new DateOnly(2026, 5, 1));
        Assert.Contains(result.Days, d => d.Date == new DateOnly(2026, 5, 31));
    }

    [Fact]
    public async Task Existing_DailyPlan_Is_Summarized()
    {
        var (service, db) = Create();
        var plan = new DailyPlan { UserId = UserA, PlanDate = new DateOnly(2026, 5, 4) };
        plan.Items.Add(new DailyPlanItem { Title = "a", SourceType = DailyPlanItemSourceType.TaskItem, IsCompleted = false });
        plan.Items.Add(new DailyPlanItem { Title = "b", SourceType = DailyPlanItemSourceType.RoutineBlock, IsCompleted = true });
        db.DailyPlans.Add(plan);
        await db.SaveChangesAsync();

        var day = (await service.GetWeekSummaryAsync(new DateOnly(2026, 5, 4))).Days.Single(x => x.Date == new DateOnly(2026, 5, 4));
        Assert.True(day.HasGeneratedDailyPlan);
        Assert.Equal(2, day.TotalPlannedItemCount);
        Assert.Equal(1, day.CompletedItemCount);
        Assert.Equal(1, day.IncompleteItemCount);
    }

    [Fact]
    public async Task NoPlan_Uses_Projected_OneOff_Count()
    {
        var (service, db) = Create();
        db.TaskItems.Add(new TaskItem { UserId = UserA, Title = "due", DueDate = new DateOnly(2026, 5, 4), TaskType = TaskType.OneOff, Status = FlowStatePlanner.Domain.Entities.TaskStatus.Planned });
        await db.SaveChangesAsync();
        var day = (await service.GetWeekSummaryAsync(new DateOnly(2026, 5, 4))).Days.Single(x => x.Date == new DateOnly(2026, 5, 4));
        Assert.False(day.HasGeneratedDailyPlan);
        Assert.Equal(1, day.DueOneOffTaskCount);
    }

    [Fact]
    public async Task Recurring_Daily_Appears_In_Projected()
    {
        var (service, db) = Create();
        db.TaskItems.Add(new TaskItem { UserId = UserA, Title = "r", TaskType = TaskType.Recurring, RecurrenceRule = "DAILY" });
        await db.SaveChangesAsync();
        var day = (await service.GetWeekSummaryAsync(new DateOnly(2026, 5, 4))).Days.Single(x => x.Date == new DateOnly(2026, 5, 4));
        Assert.Equal(1, day.RecurringTaskCount);
    }

    [Fact]
    public async Task Recurring_Weekly_Only_Matching_Day()
    {
        var (service, db) = Create();
        db.TaskItems.Add(new TaskItem { UserId = UserA, Title = "r", TaskType = TaskType.Recurring, RecurrenceRule = "WEEKLY:MON" });
        await db.SaveChangesAsync();
        var week = await service.GetWeekSummaryAsync(new DateOnly(2026, 5, 4));
        Assert.Equal(1, week.Days.Single(x => x.Date == new DateOnly(2026, 5, 4)).RecurringTaskCount);
        Assert.Equal(0, week.Days.Single(x => x.Date == new DateOnly(2026, 5, 5)).RecurringTaskCount);
    }

    [Fact]
    public async Task Routine_Blocks_Count_On_Applicable_Days()
    {
        var (service, db) = Create();
        var template = new RoutineTemplate { UserId = UserA, Name = "Morning", AppliesToDays = [DayOfWeek.Monday], IsActive = true };
        template.Blocks.Add(new RoutineBlock { Title = "Block" });
        db.RoutineTemplates.Add(template);
        await db.SaveChangesAsync();

        var week = await service.GetWeekSummaryAsync(new DateOnly(2026, 5, 4));
        Assert.Equal(1, week.Days.Single(x => x.Date == new DateOnly(2026, 5, 4)).RoutineBlockCount);
        Assert.Equal(0, week.Days.Single(x => x.Date == new DateOnly(2026, 5, 5)).RoutineBlockCount);
    }

    [Fact]
    public async Task Done_And_Archived_OneOff_Excluded()
    {
        var (service, db) = Create();
        db.TaskItems.AddRange(
            new TaskItem { UserId = UserA, Title = "done", DueDate = new DateOnly(2026, 5, 4), TaskType = TaskType.OneOff, Status = FlowStatePlanner.Domain.Entities.TaskStatus.Done },
            new TaskItem { UserId = UserA, Title = "archived", DueDate = new DateOnly(2026, 5, 4), TaskType = TaskType.OneOff, Status = FlowStatePlanner.Domain.Entities.TaskStatus.Archived });
        await db.SaveChangesAsync();

        var day = (await service.GetWeekSummaryAsync(new DateOnly(2026, 5, 4))).Days.Single(x => x.Date == new DateOnly(2026, 5, 4));
        Assert.Equal(0, day.DueOneOffTaskCount);
    }

    [Fact]
    public async Task User_Isolation_Enforced()
    {
        var (service, db) = Create();
        db.TaskItems.Add(new TaskItem { UserId = Guid.NewGuid(), Title = "other", DueDate = new DateOnly(2026, 5, 4), TaskType = TaskType.OneOff, Status = FlowStatePlanner.Domain.Entities.TaskStatus.Planned });
        await db.SaveChangesAsync();

        var day = (await service.GetWeekSummaryAsync(new DateOnly(2026, 5, 4))).Days.Single(x => x.Date == new DateOnly(2026, 5, 4));
        Assert.Equal(0, day.DueOneOffTaskCount);
    }

    [Fact]
    public async Task Invalid_Recurrence_Does_Not_Crash()
    {
        var (service, db) = Create(new ThrowingMatcher());
        db.TaskItems.Add(new TaskItem { UserId = UserA, Title = "bad", TaskType = TaskType.Recurring, RecurrenceRule = "BAD" });
        await db.SaveChangesAsync();

        var day = (await service.GetWeekSummaryAsync(new DateOnly(2026, 5, 4))).Days.Single(x => x.Date == new DateOnly(2026, 5, 4));
        Assert.Equal(0, day.RecurringTaskCount);
    }

    static readonly Guid UserA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static (CalendarSummaryService, FlowStatePlannerDbContext) Create(IRecurrenceRuleMatcher? matcher = null)
    {
        var o = new DbContextOptionsBuilder<FlowStatePlannerDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var db = new FlowStatePlannerDbContext(o);
        var s = new CalendarSummaryService(db, new FakeUser(), matcher ?? new RecurrenceRuleMatcher());
        return (s, db);
    }

    private sealed class FakeUser : ICurrentUserService { public Guid UserId => UserA; }
    private sealed class ThrowingMatcher : IRecurrenceRuleMatcher { public bool Matches(string? recurrenceRule, DateOnly date) => throw new InvalidOperationException("bad rule"); }
}
