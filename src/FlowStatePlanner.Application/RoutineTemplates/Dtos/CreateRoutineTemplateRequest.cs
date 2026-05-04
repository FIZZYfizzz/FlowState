namespace FlowStatePlanner.Application.RoutineTemplates.Dtos;

public class CreateRoutineTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<DayOfWeek> AppliesToDays { get; set; } = [];
    public bool IsActive { get; set; } = true;
    public List<RoutineBlockRequest> Blocks { get; set; } = [];
}
