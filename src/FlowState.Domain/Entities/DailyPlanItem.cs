namespace FlowState.Domain.Entities;

public class DailyPlanItem
{
    public Guid Id { get; set; }
    public Guid DailyPlanId { get; set; }
    public Guid? TaskItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public TimeOnly? StartTime { get; set; }
    public int? DurationMinutes { get; set; }
    public int SortOrder { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    public DailyPlan DailyPlan { get; set; } = null!;
    public TaskItem? TaskItem { get; set; }
}
