using FlowStatePlanner.Application.DailyPlans.Dtos;

namespace FlowStatePlanner.Application.DailyPlans;

public interface IDailyPlanService
{
    Task<DailyPlanResponse?> GetByDateAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task<DailyPlanResponse> GenerateAsync(DateOnly date, CancellationToken cancellationToken = default);
    Task<DailyPlanItemResponse?> UpdateCompletionAsync(DateOnly date, Guid itemId, bool isCompleted, CancellationToken cancellationToken = default);
}
