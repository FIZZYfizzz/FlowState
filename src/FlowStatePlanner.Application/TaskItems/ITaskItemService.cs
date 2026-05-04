using FlowStatePlanner.Application.TaskItems.Dtos;

namespace FlowStatePlanner.Application.TaskItems;

public interface ITaskItemService
{
    Task<IReadOnlyList<TaskItemResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TaskItemResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TaskItemResponse> CreateAsync(CreateTaskItemRequest request, CancellationToken cancellationToken = default);
    Task<TaskItemResponse?> UpdateAsync(Guid id, UpdateTaskItemRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
