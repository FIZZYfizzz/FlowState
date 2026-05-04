using FlowStatePlanner.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlowStatePlanner.Application.Abstractions;

public interface IApplicationDbContext
{
    DbSet<TaskItem> TaskItems { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
