namespace FlowState.Domain.Entities;

public class DailyPlan
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateOnly PlanDate { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public bool IsDeleted { get; set; }

    public User User { get; set; } = null!;
    public ICollection<DailyPlanItem> Items { get; set; } = new List<DailyPlanItem>();
}
