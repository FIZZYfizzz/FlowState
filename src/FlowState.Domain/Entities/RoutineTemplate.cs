namespace FlowState.Domain.Entities;

public class RoutineTemplate
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public bool IsDeleted { get; set; }

    public User User { get; set; } = null!;
    public ICollection<RoutineBlock> Blocks { get; set; } = new List<RoutineBlock>();
}
