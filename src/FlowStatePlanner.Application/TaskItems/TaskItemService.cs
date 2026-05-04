using FlowStatePlanner.Application.Abstractions;
using FlowStatePlanner.Application.TaskItems.Dtos;
using FlowStatePlanner.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowStatePlanner.Application.TaskItems;

public sealed class TaskItemService(IApplicationDbContext dbContext, ICurrentUserService currentUserService) : ITaskItemService
{
    private const int MaxTitleLength = 200;

    public async Task<IReadOnlyList<TaskItemResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await dbContext.TaskItems
            .Where(x => x.UserId == currentUserService.UserId && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(MapResponse())
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskItemResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.TaskItems
            .Where(x => x.Id == id && x.UserId == currentUserService.UserId && !x.IsDeleted)
            .Select(MapResponse())
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TaskItemResponse> CreateAsync(CreateTaskItemRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request);
        var entity = new TaskItem
        {
            UserId = currentUserService.UserId,
            Title = request.Title.Trim(),
            Description = request.Description,
            DueDate = request.DueDate,
            StartTime = request.StartTime,
            DurationMinutes = request.DurationMinutes,
            Priority = request.Priority,
            Status = request.Status,
            TaskType = request.TaskType,
            RecurrenceRule = request.TaskType == TaskType.Recurring ? request.RecurrenceRule?.Trim() : null
        };

        dbContext.TaskItems.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(entity);
    }

    public async Task<TaskItemResponse?> UpdateAsync(Guid id, UpdateTaskItemRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request);
        var entity = await dbContext.TaskItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == currentUserService.UserId && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;

        entity.Title = request.Title.Trim();
        entity.Description = request.Description;
        entity.DueDate = request.DueDate;
        entity.StartTime = request.StartTime;
        entity.DurationMinutes = request.DurationMinutes;
        entity.Priority = request.Priority;
        entity.Status = request.Status;
        entity.TaskType = request.TaskType;
        entity.RecurrenceRule = request.TaskType == TaskType.Recurring ? request.RecurrenceRule?.Trim() : null;

        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.TaskItems.FirstOrDefaultAsync(x => x.Id == id && x.UserId == currentUserService.UserId && !x.IsDeleted, cancellationToken);
        if (entity is null) return false;
        entity.IsDeleted = true;
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static TaskItemResponse ToResponse(TaskItem taskItem) => new()
    {
        Id = taskItem.Id,
        Title = taskItem.Title,
        Description = taskItem.Description,
        DueDate = taskItem.DueDate,
        StartTime = taskItem.StartTime,
        DurationMinutes = taskItem.DurationMinutes,
        Priority = taskItem.Priority,
        Status = taskItem.Status,
        TaskType = taskItem.TaskType,
        RecurrenceRule = taskItem.RecurrenceRule
    };

    private static System.Linq.Expressions.Expression<Func<TaskItem, TaskItemResponse>> MapResponse() => taskItem => new TaskItemResponse
    {
        Id = taskItem.Id,
        Title = taskItem.Title,
        Description = taskItem.Description,
        DueDate = taskItem.DueDate,
        StartTime = taskItem.StartTime,
        DurationMinutes = taskItem.DurationMinutes,
        Priority = taskItem.Priority,
        Status = taskItem.Status,
        TaskType = taskItem.TaskType,
        RecurrenceRule = taskItem.RecurrenceRule
    };

    private static void Validate(CreateTaskItemRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || request.Title.Trim().Length > MaxTitleLength)
            throw new ArgumentException($"Title is required and must be at most {MaxTitleLength} characters.");

        if (request.DurationMinutes.HasValue && request.DurationMinutes.Value <= 0)
            throw new ArgumentException("DurationMinutes must be positive when provided.");

        if (request.TaskType == TaskType.Recurring && string.IsNullOrWhiteSpace(request.RecurrenceRule))
            throw new ArgumentException("RecurrenceRule is required for recurring tasks.");
    }
}
