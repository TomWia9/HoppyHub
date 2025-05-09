using System.Reflection;
using Azure.Monitor.OpenTelemetry.AspNetCore;
using MassTransit;
using Serilog;
using SharedUtilities.Filters;

namespace App;

/// <summary>
///     The ConfigureServices class.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    ///     Adds services.
    /// </summary>
    /// <param name="services">The services</param>
    /// <param name="configuration">The configuration</param>
    /// <param name="environment">The environment</param>
    public static void AddServices(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddSerilog();
        services.AddMassTransit(x =>
        {
            x.AddConsumers(Assembly.GetExecutingAssembly());

            if (environment.IsDevelopment())
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration.GetValue<string>("RabbitMQ:Host"), "/", h =>
                    {
                        h.Username(configuration.GetValue<string>("RabbitMQ:Username") ?? "");
                        h.Password(configuration.GetValue<string>("RabbitMQ:Password") ?? "");
                    });

                    cfg.ConfigureEndpoints(context,
                        endpointNameFormatter: new DefaultEndpointNameFormatter(prefix: "EmailManagement"));
                    cfg.UseConsumeFilter(typeof(MessageValidationFilter<>), context);
                });
            }
            else
            {
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(configuration.GetConnectionString("AzureServiceBusConnection"));
                    cfg.ConfigureEndpoints(context,
                        endpointNameFormatter: new DefaultEndpointNameFormatter(prefix: "BeerManagement"));
                    cfg.UseConsumeFilter(typeof(MessageValidationFilter<>), context);
                });
            }
        });
        if (!environment.IsDevelopment())
        {
            services.AddOpenTelemetry().UseAzureMonitor();
        }
    }
}