using System.Reflection;
using Application.Common.Interfaces;
using Application.Common.Services;
using Application.Opinions.Queries.GetOpinions;
using Application.Services;
using Domain.Entities;
using FluentValidation;
using MassTransit;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using SharedUtilities;
using SharedUtilities.Behaviors;
using SharedUtilities.Filters;

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
        services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(SharedUtilitiesAssemblyMarker)));
        services.AddTransient(typeof(IRequestPreProcessor<>), typeof(LoggingBehavior<>));
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        services.AddMassTransit(x =>
        {
            x.AddConsumers(Assembly.GetExecutingAssembly());
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
                cfg.UseConsumeFilter(typeof(MessageValidationFilter<>), context);
            });
        });

        services.AddTransient<IOpinionsService, OpinionsService>(); //TODO: Add unit test
        services.AddTransient(typeof(IQueryService<>), typeof(QueryService<>));
        services.AddTransient<IFilteringHelper<Opinion, GetOpinionsQuery>, OpinionsFilteringHelper>();

        return services;
    }
}