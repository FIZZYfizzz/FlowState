using Xunit;
using FlowStatePlanner.Application.Abstractions;
using FlowStatePlanner.Application.TaskItems;
using FlowStatePlanner.Application.TaskItems.Dtos;
using FlowStatePlanner.Domain.Entities;
using FlowStatePlanner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FlowStatePlanner.Tests;

public class TaskItemServiceTests
{
    [Fact]
    public async Task Create_ValidTask_Succeeds()
    {
        var service = CreateService();
        var task = await service.CreateAsync(new CreateTaskItemRequest { Title = "Task A", DurationMinutes = 30 });
        Assert.Equal("Task A", task.Title);
        Assert.Equal(30, task.DurationMinutes);
    }

    [Fact]
    public async Task Create_InvalidTitle_Throws()
    {
        var service = CreateService();
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(new CreateTaskItemRequest { Title = "   " }));
    }

    [Fact]
    public async Task Create_RecurringWithoutRule_Throws()
    {
        var service = CreateService();
        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(new CreateTaskItemRequest { Title = "R", TaskType = TaskType.Recurring }));
    }

    [Fact]
    public async Task GetById_OtherUserTask_ReturnsNull()
    {
        var (service, db) = CreateServiceWithDb();
        db.TaskItems.Add(new TaskItem { Title = "X", UserId = Guid.NewGuid() });
        await db.SaveChangesAsync();
        var item = await db.TaskItems.FirstAsync();

        var result = await service.GetByIdAsync(item.Id);
        Assert.Null(result);
    }

    [Fact]
    public async Task Delete_SoftDelete_HidesTask()
    {
        var (service, db) = CreateServiceWithDb();
        var created = await service.CreateAsync(new CreateTaskItemRequest { Title = "Delete me" });

        var deleted = await service.DeleteAsync(created.Id);
        var fetched = await service.GetByIdAsync(created.Id);
        var entity = await db.TaskItems.FirstAsync(x => x.Id == created.Id);

        Assert.True(deleted);
        Assert.Null(fetched);
        Assert.True(entity.IsDeleted);
    }

    private static TaskItemService CreateService()
    {
        var (service, _) = CreateServiceWithDb();
        return service;
    }

    private static (TaskItemService service, FlowStatePlannerDbContext dbContext) CreateServiceWithDb()
    {
        var options = new DbContextOptionsBuilder<FlowStatePlannerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new FlowStatePlannerDbContext(options);
        var currentUser = new FakeCurrentUserService();
        var service = new TaskItemService(db, currentUser);
        return (service, db);
    }

    private sealed class FakeCurrentUserService : ICurrentUserService
    {
        public Guid UserId { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    }
}
