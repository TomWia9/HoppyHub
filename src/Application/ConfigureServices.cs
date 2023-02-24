using System.Reflection;
using Application.Common.Behaviours;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

/// <summary>
///     The ConfigureServices class
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    ///     Adds application project services
    /// </summary>
    /// <param name="services">The services</param>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehaviour<,>));
        });

        return services;
    }
}