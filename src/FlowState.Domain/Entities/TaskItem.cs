namespace FlowState.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateOnly? DueDate { get; set; }
    public TimeOnly? StartTime { get; set; }
    public int? DurationMinutes { get; set; }
    public TaskType TaskType { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public bool IsDeleted { get; set; }

    public User User { get; set; } = null!;
}
