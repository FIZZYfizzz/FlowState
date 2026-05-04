namespace FlowStatePlanner.Domain.Entities;

public class DailyPlanItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DailyPlanId { get; set; }
    public Guid? TaskItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public TimeOnly? PlannedStartTime { get; set; }
    public TimeOnly? PlannedEndTime { get; set; }
    public short SortOrder { get; set; }
    public bool IsCompleted { get; set; }

    public DailyPlan DailyPlan { get; set; } = null!;
    public TaskItem? TaskItem { get; set; }
}
