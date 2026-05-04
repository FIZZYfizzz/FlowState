using FlowStatePlanner.Application.Calendar;
using FlowStatePlanner.Application.DailyPlans;
using FlowStatePlanner.Application.RoutineTemplates;
using FlowStatePlanner.Application.TaskItems;
using Microsoft.Extensions.DependencyInjection;

namespace FlowStatePlanner.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITaskItemService, TaskItemService>();
        services.AddScoped<IRoutineTemplateService, RoutineTemplateService>();
        services.AddScoped<IRecurrenceRuleMatcher, RecurrenceRuleMatcher>();
        services.AddScoped<IDailyPlanService, DailyPlanService>();
        services.AddScoped<ICalendarSummaryService, CalendarSummaryService>();
        return services;
    }
}
