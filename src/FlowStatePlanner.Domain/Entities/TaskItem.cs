namespace FlowStatePlanner.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateOnly? DueDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public int? DurationMinutes { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public TaskStatus Status { get; set; } = TaskStatus.Backlog;
    public TaskType TaskType { get; set; } = TaskType.OneOff;
    public string? RecurrenceRule { get; set; }
    public bool IsDeleted { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public User User { get; set; } = null!;
}

public enum TaskPriority { Low = 1, Medium = 2, High = 3 }
public enum TaskStatus { Backlog = 1, Planned = 2, InProgress = 3, Done = 4, Archived = 5 }
public enum TaskType { OneOff = 1, Recurring = 2 }
