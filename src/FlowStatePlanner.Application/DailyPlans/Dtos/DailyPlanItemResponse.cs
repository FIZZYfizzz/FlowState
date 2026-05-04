using FlowStatePlanner.Domain.Entities;

namespace FlowStatePlanner.Application.DailyPlans.Dtos;

public sealed class DailyPlanItemResponse
{
    public Guid Id { get; set; }
    public DailyPlanItemSourceType SourceType { get; set; }
    public Guid? RoutineBlockId { get; set; }
    public Guid? TaskItemId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TimeOnly? PlannedStartTime { get; set; }
    public TimeOnly? PlannedEndTime { get; set; }
    public int SortOrder { get; set; }
    public bool IsCompleted { get; set; }
}
