namespace FlowStatePlanner.Application.RoutineTemplates.Dtos;

public sealed class RoutineTemplateResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<DayOfWeek> AppliesToDays { get; set; } = [];
    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset UpdatedAtUtc { get; set; }
    public IReadOnlyList<RoutineBlockResponse> Blocks { get; set; } = [];
}
