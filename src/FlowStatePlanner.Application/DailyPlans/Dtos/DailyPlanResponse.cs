namespace FlowStatePlanner.Application.DailyPlans.Dtos;

public sealed class DailyPlanResponse
{
    public Guid Id { get; set; }
    public DateOnly PlanDate { get; set; }
    public IReadOnlyList<DailyPlanItemResponse> Items { get; set; } = [];
}
