using FlowStatePlanner.Application.Abstractions;
using FlowStatePlanner.Application.RoutineTemplates.Dtos;
using FlowStatePlanner.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowStatePlanner.Application.RoutineTemplates;

public sealed class RoutineTemplateService(IApplicationDbContext dbContext, ICurrentUserService currentUserService) : IRoutineTemplateService
{
    public async Task<IReadOnlyList<RoutineTemplateResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var templates = await QueryTemplates().ToListAsync(cancellationToken);
        return templates.Select(Map).ToList();
    }

    public async Task<RoutineTemplateResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await QueryTemplates().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        return entity is null ? null : Map(entity);
    }

    public async Task<RoutineTemplateResponse> CreateAsync(CreateRoutineTemplateRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request);
        var now = DateTimeOffset.UtcNow;
        var entity = new RoutineTemplate { UserId = currentUserService.UserId, Name = request.Name.Trim(), Description = request.Description, AppliesToDays = request.AppliesToDays.Distinct().ToList(), IsActive = request.IsActive, CreatedAtUtc = now, UpdatedAtUtc = now };
        entity.Blocks = request.Blocks.Select(b => new RoutineBlock { Title = b.Title.Trim(), Description = b.Description, StartTime = b.StartTime, EndTime = b.EndTime, SortOrder = b.SortOrder, FlexibilityType = b.FlexibilityType, CreatedAtUtc = now, UpdatedAtUtc = now }).ToList();
        dbContext.RoutineTemplates.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<RoutineTemplateResponse?> UpdateAsync(Guid id, UpdateRoutineTemplateRequest request, CancellationToken cancellationToken = default)
    {
        Validate(request);
        var entity = await dbContext.RoutineTemplates.Include(x => x.Blocks).FirstOrDefaultAsync(x => x.Id == id && x.UserId == currentUserService.UserId && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        var now = DateTimeOffset.UtcNow;
        entity.Name = request.Name.Trim(); entity.Description = request.Description; entity.AppliesToDays = request.AppliesToDays.Distinct().ToList(); entity.IsActive = request.IsActive; entity.UpdatedAtUtc = now;
        var requestedIds = request.Blocks.Where(x => x.Id.HasValue).Select(x => x.Id!.Value).ToHashSet();
        foreach (var block in entity.Blocks.Where(x => !x.IsDeleted && !requestedIds.Contains(x.Id))) { block.IsDeleted = true; block.UpdatedAtUtc = now; }
        foreach (var requestBlock in request.Blocks)
        {
            var existing = requestBlock.Id.HasValue ? entity.Blocks.FirstOrDefault(x => x.Id == requestBlock.Id.Value) : null;
            if (existing is null)
            {
                entity.Blocks.Add(new RoutineBlock { Title = requestBlock.Title.Trim(), Description = requestBlock.Description, StartTime = requestBlock.StartTime, EndTime = requestBlock.EndTime, SortOrder = requestBlock.SortOrder, FlexibilityType = requestBlock.FlexibilityType, CreatedAtUtc = now, UpdatedAtUtc = now });
            }
            else
            {
                existing.IsDeleted = false; existing.Title = requestBlock.Title.Trim(); existing.Description = requestBlock.Description; existing.StartTime = requestBlock.StartTime; existing.EndTime = requestBlock.EndTime; existing.SortOrder = requestBlock.SortOrder; existing.FlexibilityType = requestBlock.FlexibilityType; existing.UpdatedAtUtc = now;
            }
        }
        await dbContext.SaveChangesAsync(cancellationToken);
        return Map(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.RoutineTemplates.Include(x => x.Blocks).FirstOrDefaultAsync(x => x.Id == id && x.UserId == currentUserService.UserId && !x.IsDeleted, cancellationToken);
        if (entity is null) return false;
        entity.IsDeleted = true; entity.UpdatedAtUtc = DateTimeOffset.UtcNow;
        foreach (var block in entity.Blocks.Where(x => !x.IsDeleted)) { block.IsDeleted = true; block.UpdatedAtUtc = entity.UpdatedAtUtc; }
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    private IQueryable<RoutineTemplate> QueryTemplates() => dbContext.RoutineTemplates.Include(x => x.Blocks).Where(x => x.UserId == currentUserService.UserId && !x.IsDeleted);

    private static RoutineTemplateResponse Map(RoutineTemplate x) => new() { Id = x.Id, UserId = x.UserId, Name = x.Name, Description = x.Description, AppliesToDays = x.AppliesToDays, IsActive = x.IsActive, CreatedAtUtc = x.CreatedAtUtc, UpdatedAtUtc = x.UpdatedAtUtc, Blocks = x.Blocks.Where(b => !b.IsDeleted).OrderBy(b => b.SortOrder).Select(b => new RoutineBlockResponse { Id = b.Id, Title = b.Title, Description = b.Description, StartTime = b.StartTime, EndTime = b.EndTime, SortOrder = b.SortOrder, FlexibilityType = b.FlexibilityType }).ToList() };

    private static void Validate(CreateRoutineTemplateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.Name.Trim().Length > 100) throw new ArgumentException("Template name is required and must be at most 100 characters.");
        if (request.IsActive && request.AppliesToDays.Count == 0) throw new ArgumentException("At least one appliesToDay is required for active templates.");
        foreach (var block in request.Blocks)
        {
            if (string.IsNullOrWhiteSpace(block.Title) || block.Title.Trim().Length > 200) throw new ArgumentException("Block title is required and must be at most 200 characters.");
            if (block.SortOrder < 0) throw new ArgumentException("SortOrder must be non-negative.");
            if (block.StartTime.HasValue && block.EndTime.HasValue && block.EndTime <= block.StartTime) throw new ArgumentException("Block endTime must be after startTime when both are provided.");
            if (!Enum.IsDefined(block.FlexibilityType)) throw new ArgumentException("FlexibilityType is required.");
        }
    }
}
