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
        return services;
    }
}
