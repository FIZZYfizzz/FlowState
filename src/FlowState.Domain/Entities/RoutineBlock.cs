namespace FlowState.Domain.Entities;

public class RoutineBlock
{
    public Guid Id { get; set; }
    public Guid RoutineTemplateId { get; set; }
    public string Title { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public int DurationMinutes { get; set; }
    public int SortOrder { get; set; }
    public FlexibilityType FlexibilityType { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public bool IsDeleted { get; set; }

    public RoutineTemplate RoutineTemplate { get; set; } = null!;
}
