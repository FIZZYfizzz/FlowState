namespace FlowStatePlanner.Domain.Entities;

public class RoutineBlock
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RoutineTemplateId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public int SortOrder { get; set; }
    public RoutineBlockFlexibilityType FlexibilityType { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public bool IsDeleted { get; set; }

    public RoutineTemplate RoutineTemplate { get; set; } = null!;
}

public enum RoutineBlockFlexibilityType
{
    Fixed = 1,
    Flexible = 2,
    Optional = 3
}
