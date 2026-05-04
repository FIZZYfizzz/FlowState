using FlowStatePlanner.Application.Abstractions;
using FlowStatePlanner.Application.RoutineTemplates;
using FlowStatePlanner.Application.RoutineTemplates.Dtos;
using FlowStatePlanner.Domain.Entities;
using FlowStatePlanner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FlowStatePlanner.Tests;

public class RoutineTemplateServiceTests
{
    [Fact]
    public async Task Create_ValidTemplateWithBlocks_Succeeds()
    {
        var service = CreateService();
        var result = await service.CreateAsync(new CreateRoutineTemplateRequest
        {
            Name = "Weekday",
            IsActive = true,
            AppliesToDays = [DayOfWeek.Monday],
            Blocks = [new RoutineBlockRequest { Title = "Focus", SortOrder = 0, FlexibilityType = RoutineBlockFlexibilityType.Fixed }]
        });

        Assert.Equal("Weekday", result.Name);
        Assert.Single(result.Blocks);
    }

    [Fact]
    public async Task Create_MissingTemplateName_Throws()
        => await Assert.ThrowsAsync<ArgumentException>(() => CreateService().CreateAsync(new CreateRoutineTemplateRequest { Name = "   ", AppliesToDays = [DayOfWeek.Monday] }));

    [Fact]
    public async Task Create_BlockEndTimeBeforeStart_Throws()
        => await Assert.ThrowsAsync<ArgumentException>(() => CreateService().CreateAsync(new CreateRoutineTemplateRequest { Name = "A", AppliesToDays = [DayOfWeek.Monday], Blocks = [new RoutineBlockRequest { Title = "B", StartTime = new TimeOnly(10,0), EndTime = new TimeOnly(9,0), SortOrder = 0, FlexibilityType = RoutineBlockFlexibilityType.Fixed }] }));

    [Fact]
    public async Task GetById_ReturnsBlocksOrderedBySortOrder()
    {
        var service = CreateService();
        var created = await service.CreateAsync(new CreateRoutineTemplateRequest { Name = "A", AppliesToDays = [DayOfWeek.Monday], Blocks = [new RoutineBlockRequest { Title = "second", SortOrder = 2, FlexibilityType = RoutineBlockFlexibilityType.Fixed }, new RoutineBlockRequest { Title = "first", SortOrder = 1, FlexibilityType = RoutineBlockFlexibilityType.Flexible }] });
        var fetched = await service.GetByIdAsync(created.Id);
        Assert.Equal(new[] { "first", "second" }, fetched!.Blocks.Select(x => x.Title).ToArray());
    }

    [Fact]
    public async Task GetById_OtherUserTemplate_ReturnsNull()
    {
        var (service, db) = CreateServiceWithDb();
        db.RoutineTemplates.Add(new RoutineTemplate { Name = "X", UserId = Guid.NewGuid(), AppliesToDays = [DayOfWeek.Monday] });
        await db.SaveChangesAsync();
        var id = await db.RoutineTemplates.Select(x => x.Id).FirstAsync();
        Assert.Null(await service.GetByIdAsync(id));
    }

    [Fact]
    public async Task Delete_SoftDelete_HidesTemplateAndBlocks()
    {
        var (service, db) = CreateServiceWithDb();
        var created = await service.CreateAsync(new CreateRoutineTemplateRequest { Name = "Delete", AppliesToDays = [DayOfWeek.Monday], Blocks = [new RoutineBlockRequest { Title = "B", SortOrder = 0, FlexibilityType = RoutineBlockFlexibilityType.Optional }] });
        var deleted = await service.DeleteAsync(created.Id);
        var fetched = await service.GetByIdAsync(created.Id);
        var entity = await db.RoutineTemplates.Include(x => x.Blocks).FirstAsync(x => x.Id == created.Id);
        Assert.True(deleted);
        Assert.Null(fetched);
        Assert.True(entity.IsDeleted);
        Assert.All(entity.Blocks, b => Assert.True(b.IsDeleted));
    }

    private static RoutineTemplateService CreateService() => CreateServiceWithDb().service;

    private static (RoutineTemplateService service, FlowStatePlannerDbContext dbContext) CreateServiceWithDb()
    {
        var options = new DbContextOptionsBuilder<FlowStatePlannerDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var db = new FlowStatePlannerDbContext(options);
        return (new RoutineTemplateService(db, new FakeCurrentUserService()), db);
    }

    private sealed class FakeCurrentUserService : ICurrentUserService
    {
        public Guid UserId { get; } = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    }
}
