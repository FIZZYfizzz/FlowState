namespace FlowStatePlanner.Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
    public ICollection<RoutineTemplate> RoutineTemplates { get; set; } = new List<RoutineTemplate>();
    public ICollection<DailyPlan> DailyPlans { get; set; } = new List<DailyPlan>();
}
