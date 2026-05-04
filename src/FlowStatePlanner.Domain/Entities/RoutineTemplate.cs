namespace FlowStatePlanner.Domain.Entities;

public class RoutineTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<DayOfWeek> AppliesToDays { get; set; } = [];
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public bool IsDeleted { get; set; }

    public User User { get; set; } = null!;
    public ICollection<RoutineBlock> Blocks { get; set; } = new List<RoutineBlock>();
}
