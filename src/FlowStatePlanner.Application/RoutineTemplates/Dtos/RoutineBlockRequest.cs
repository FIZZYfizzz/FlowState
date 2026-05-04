using FlowStatePlanner.Domain.Entities;

namespace FlowStatePlanner.Application.RoutineTemplates.Dtos;

public sealed class RoutineBlockRequest
{
    public Guid? Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public int SortOrder { get; set; }
    public RoutineBlockFlexibilityType FlexibilityType { get; set; }
}
