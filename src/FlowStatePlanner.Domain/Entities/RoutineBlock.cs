namespace FlowStatePlanner.Domain.Entities;

public class RoutineBlock
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RoutineTemplateId { get; set; }
    public string Title { get; set; } = string.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public short SortOrder { get; set; }

    public RoutineTemplate RoutineTemplate { get; set; } = null!;
}
