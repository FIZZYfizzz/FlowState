using FlowStatePlanner.Application.TaskItems;
using Microsoft.Extensions.DependencyInjection;

namespace FlowStatePlanner.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITaskItemService, TaskItemService>();
        return services;
    }
}
