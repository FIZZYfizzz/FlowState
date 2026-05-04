using FlowStatePlanner.Application.Abstractions;
using FlowStatePlanner.Infrastructure.Identity;
using FlowStatePlanner.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FlowStatePlanner.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Connection string 'Postgres' was not found.");

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, DevelopmentCurrentUserService>();

        services.AddDbContext<FlowStatePlannerDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "planner")));

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<FlowStatePlannerDbContext>());

        return services;
    }
}
