using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Observa.Domain.Abstractions;
using Observa.Domain.Repositories;
using Observa.Infrastructure.Caching;
using Observa.Infrastructure.Persistence;
using Observa.Infrastructure.Persistence.Repositories;
using Observa.Application.Abstractions.Notifications;
using Observa.Infrastructure.RealTime;
using Observa.Infrastructure.Simulation;

namespace Observa.Infrastructure;

/// <summary>
/// Metodo de extension para registrar los servicios de la capa de infraestructura.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddCaching(configuration);
        services.AddRealTime();

        return services;
    }

    private static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<ObservaDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ObservaDbContext>());
        services.AddScoped<IDashboardRepository, DashboardRepository>();
        services.AddScoped<IAlertRepository, AlertRepository>();
        services.AddScoped<IDataSourceRepository, DataSourceRepository>();
    }

    private static void AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis");

        if (!string.IsNullOrWhiteSpace(redisConnectionString))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "observa:";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        services.AddScoped<RedisCacheService>();
    }

    private static void AddRealTime(this IServiceCollection services)
    {
        services.AddScoped<IDashboardNotificationService, DashboardNotificationService>();
        services.AddHostedService<DataSimulationService>();
    }
}
