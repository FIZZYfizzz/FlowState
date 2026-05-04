namespace FlowState.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    public ICollection<RoutineTemplate> RoutineTemplates { get; set; } = new List<RoutineTemplate>();
    public ICollection<DailyPlan> DailyPlans { get; set; } = new List<DailyPlan>();
}
