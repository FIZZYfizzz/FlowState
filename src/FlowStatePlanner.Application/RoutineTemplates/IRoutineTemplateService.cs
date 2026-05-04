using FlowStatePlanner.Application.RoutineTemplates.Dtos;

namespace FlowStatePlanner.Application.RoutineTemplates;

public interface IRoutineTemplateService
{
    Task<IReadOnlyList<RoutineTemplateResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<RoutineTemplateResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<RoutineTemplateResponse> CreateAsync(CreateRoutineTemplateRequest request, CancellationToken cancellationToken = default);
    Task<RoutineTemplateResponse?> UpdateAsync(Guid id, UpdateRoutineTemplateRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
