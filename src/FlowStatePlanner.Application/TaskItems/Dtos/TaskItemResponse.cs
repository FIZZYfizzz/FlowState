using FlowStatePlanner.Domain.Entities;

namespace FlowStatePlanner.Application.TaskItems.Dtos;

public sealed class TaskItemResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly? DueDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public int? DurationMinutes { get; set; }
    public TaskPriority Priority { get; set; }
    public FlowStatePlanner.Domain.Entities.TaskStatus Status { get; set; }
    public TaskType TaskType { get; set; }
    public string? RecurrenceRule { get; set; }
}
