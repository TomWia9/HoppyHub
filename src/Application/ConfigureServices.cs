using System.Reflection;
using Application.Beers.Services;
using Application.Common.Behaviors;
using Application.Common.Interfaces;
using Application.Common.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

/// <summary>
///     The ConfigureServices class.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    ///     Adds application project services.
    /// </summary>
    /// <param name="services">The services</param>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });
        
        services.AddTransient(typeof(IQueryService<>), typeof(QueryService<>));
        services.AddTransient<IBeersService, BeersService>();

        return services;
    }
}