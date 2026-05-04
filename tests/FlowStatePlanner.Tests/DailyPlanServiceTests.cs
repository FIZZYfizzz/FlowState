using FlowStatePlanner.Application.Abstractions;
using FlowStatePlanner.Application.DailyPlans;
using FlowStatePlanner.Domain.Entities;
using FlowStatePlanner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FlowStatePlanner.Tests;

public class DailyPlanServiceTests
{
    [Fact] public async Task Generates_FromRoutineBlocks() { var (s,db)=Create(); SeedRoutine(db); var plan=await s.GenerateAsync(new DateOnly(2026,5,4)); Assert.Contains(plan.Items,x=>x.SourceType==DailyPlanItemSourceType.RoutineBlock); }
    [Fact] public async Task Includes_OneOff_DueDate() { var (s,db)=Create(); db.TaskItems.Add(new TaskItem{UserId=UserA,Title="t",DueDate=new DateOnly(2026,5,4)}); await db.SaveChangesAsync(); var p=await s.GenerateAsync(new DateOnly(2026,5,4)); Assert.Contains(p.Items,x=>x.Title=="t"); }
    [Fact] public async Task Includes_Recurring_Daily() { var (s,db)=Create(); db.TaskItems.Add(new TaskItem{UserId=UserA,Title="r",TaskType=TaskType.Recurring,RecurrenceRule="DAILY"}); await db.SaveChangesAsync(); var p=await s.GenerateAsync(new DateOnly(2026,5,4)); Assert.Contains(p.Items,x=>x.Title=="r"); }
    [Fact] public async Task Includes_Recurring_Weekly_MatchingOnly() { var (s,db)=Create(); db.TaskItems.Add(new TaskItem{UserId=UserA,Title="w",TaskType=TaskType.Recurring,RecurrenceRule="WEEKLY:MON"}); await db.SaveChangesAsync(); Assert.Contains((await s.GenerateAsync(new DateOnly(2026,5,4))).Items,x=>x.Title=="w"); Assert.DoesNotContain((await s.GenerateAsync(new DateOnly(2026,5,5))).Items,x=>x.Title=="w"); }
    [Fact] public async Task Idempotent_GenerateTwice_NoDuplicates() { var (s,db)=Create(); db.TaskItems.Add(new TaskItem{UserId=UserA,Title="t",DueDate=new DateOnly(2026,5,4)}); await db.SaveChangesAsync(); var p1=await s.GenerateAsync(new DateOnly(2026,5,4)); var p2=await s.GenerateAsync(new DateOnly(2026,5,4)); Assert.Equal(p1.Id,p2.Id); Assert.Single(p2.Items.Where(x=>x.Title=="t")); }
    [Fact] public async Task CarryForward_Incomplete_OneOffOnly() { var (s,db)=Create(); var t=new TaskItem{UserId=UserA,Title="carry",TaskType=TaskType.OneOff}; db.TaskItems.Add(t); var prev=new DailyPlan{UserId=UserA,PlanDate=new DateOnly(2026,5,3)}; prev.Items.Add(new DailyPlanItem{SourceType=DailyPlanItemSourceType.TaskItem,TaskItem=t,Title=t.Title,IsCompleted=false}); db.DailyPlans.Add(prev); await db.SaveChangesAsync(); var p=await s.GenerateAsync(new DateOnly(2026,5,4)); Assert.Contains(p.Items,x=>x.SourceType==DailyPlanItemSourceType.CarryForward&&x.Title=="carry"); }
    [Fact] public async Task CarryForward_Excludes_Completed() { var (s,db)=Create(); var t=new TaskItem{UserId=UserA,Title="done",TaskType=TaskType.OneOff}; db.TaskItems.Add(t); var prev=new DailyPlan{UserId=UserA,PlanDate=new DateOnly(2026,5,3)}; prev.Items.Add(new DailyPlanItem{SourceType=DailyPlanItemSourceType.TaskItem,TaskItem=t,Title=t.Title,IsCompleted=true}); db.DailyPlans.Add(prev); await db.SaveChangesAsync(); var p=await s.GenerateAsync(new DateOnly(2026,5,4)); Assert.DoesNotContain(p.Items,x=>x.Title=="done"); }
    [Fact] public async Task UserIsolation() { var (s,db)=Create(); db.DailyPlans.Add(new DailyPlan{UserId=Guid.NewGuid(),PlanDate=new DateOnly(2026,5,4)}); await db.SaveChangesAsync(); Assert.Null(await s.GetByDateAsync(new DateOnly(2026,5,4))); }
    [Fact] public async Task Completion_Updates_Item() { var (s,db)=Create(); db.TaskItems.Add(new TaskItem{UserId=UserA,Title="x",DueDate=new DateOnly(2026,5,4)}); await db.SaveChangesAsync(); var p=await s.GenerateAsync(new DateOnly(2026,5,4)); var item=p.Items.First(); var updated=await s.UpdateCompletionAsync(new DateOnly(2026,5,4),item.Id,true); Assert.True(updated!.IsCompleted); }
    [Fact] public async Task Completion_OneOff_MarksTaskDone() { var (s,db)=Create(); var t=new TaskItem{UserId=UserA,Title="one",DueDate=new DateOnly(2026,5,4)}; db.TaskItems.Add(t); await db.SaveChangesAsync(); var p=await s.GenerateAsync(new DateOnly(2026,5,4)); var item=p.Items.First(x=>x.TaskItemId==t.Id); await s.UpdateCompletionAsync(new DateOnly(2026,5,4),item.Id,true); Assert.Equal(FlowStatePlanner.Domain.Entities.TaskStatus.Done,(await db.TaskItems.FirstAsync(x=>x.Id==t.Id)).Status); }
    [Fact] public async Task Completion_Recurring_DoesNotMarkDone() { var (s,db)=Create(); var t=new TaskItem{UserId=UserA,Title="rec",TaskType=TaskType.Recurring,RecurrenceRule="DAILY"}; db.TaskItems.Add(t); await db.SaveChangesAsync(); var p=await s.GenerateAsync(new DateOnly(2026,5,4)); var item=p.Items.First(x=>x.TaskItemId==t.Id); await s.UpdateCompletionAsync(new DateOnly(2026,5,4),item.Id,true); Assert.NotEqual(FlowStatePlanner.Domain.Entities.TaskStatus.Done,(await db.TaskItems.FirstAsync(x=>x.Id==t.Id)).Status); }

    static readonly Guid UserA = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static (DailyPlanService, FlowStatePlannerDbContext) Create() { var o=new DbContextOptionsBuilder<FlowStatePlannerDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options; var db=new FlowStatePlannerDbContext(o); var s=new DailyPlanService(db,new FakeUser(),new RecurrenceRuleMatcher()); return (s,db); }
    private static void SeedRoutine(FlowStatePlannerDbContext db){ var rt=new RoutineTemplate{UserId=UserA,Name="R",AppliesToDays=[DayOfWeek.Monday],IsActive=true}; rt.Blocks.Add(new RoutineBlock{Title="Block",SortOrder=1}); db.RoutineTemplates.Add(rt); db.SaveChanges(); }
    private sealed class FakeUser : ICurrentUserService { public Guid UserId => UserA; }
}
