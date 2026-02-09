using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Observa.Api.Endpoints;
using Observa.Api.Middleware;
using Observa.Application;
using Observa.Infrastructure;
using Observa.Infrastructure.Persistence;
using Observa.Infrastructure.RealTime;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddSignalR();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ObservaDbContext>();
        dbContext.Database.Migrate();
    }

    app.UseExceptionHandler();
    app.UseSerilogRequestLogging();

    app.MapGet("/health", () => Results.Ok(new { Status = "Healthy" }))
        .WithName("HealthCheck")
        .WithTags("System");

    app.MapDashboardEndpoints();
    app.MapDataSourceEndpoints();
    app.MapHub<DashboardHub>("/hubs/dashboard");

    app.Run();
}
finally
{
    Log.CloseAndFlush();
}
