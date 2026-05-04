using FlowStatePlanner.Domain.Entities;

namespace FlowStatePlanner.Application.TaskItems.Dtos;

public class CreateTaskItemRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly? DueDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public int? DurationMinutes { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public FlowStatePlanner.Domain.Entities.TaskStatus Status { get; set; } = FlowStatePlanner.Domain.Entities.TaskStatus.Backlog;
    public TaskType TaskType { get; set; } = TaskType.OneOff;
    public string? RecurrenceRule { get; set; }
}
