namespace FlowStatePlanner.Domain.Entities;

public class DailyPlanItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DailyPlanId { get; set; }
    public DailyPlanItemSourceType SourceType { get; set; }
    public Guid? RoutineBlockId { get; set; }
    public Guid? TaskItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TimeOnly? PlannedStartTime { get; set; }
    public TimeOnly? PlannedEndTime { get; set; }
    public int SortOrder { get; set; }
    public bool IsCompleted { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public bool IsDeleted { get; set; }

    public DailyPlan DailyPlan { get; set; } = null!;
    public RoutineBlock? RoutineBlock { get; set; }
    public TaskItem? TaskItem { get; set; }
}

public enum DailyPlanItemSourceType
{
    RoutineBlock = 1,
    TaskItem = 2,
    CarryForward = 3
}
