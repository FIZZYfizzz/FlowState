namespace FlowStatePlanner.Domain.Entities;

public class DailyPlan
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public DateOnly PlanDate { get; set; }
    public PlanGenerationSource GenerationSource { get; set; } = PlanGenerationSource.Manual;
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public User User { get; set; } = null!;
    public ICollection<DailyPlanItem> Items { get; set; } = new List<DailyPlanItem>();
}

public enum PlanGenerationSource { Manual = 1, RoutineTemplate = 2, Hybrid = 3 }
