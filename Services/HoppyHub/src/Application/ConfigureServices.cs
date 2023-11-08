using System.Reflection;
using Application.Beers.Queries.GetBeers;
using Application.Beers.Services;
using Application.BeerStyles.Queries.GetBeerStyles;
using Application.Breweries.Queries.GetBreweries;
using Application.Common.Interfaces;
using Application.Common.Services;
using Application.Favorites.Queries.GetFavorites;
using Application.Opinions.Queries.GetOpinions;
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

        services.AddTransient(typeof(IQueryService<>), typeof(QueryService<>));

        services.AddTransient<IFilteringHelper<Brewery, GetBreweriesQuery>, BreweriesFilteringHelper>();
        services.AddTransient<IFilteringHelper<Beer, GetBeersQuery>, BeersFilteringHelper>();
        services.AddTransient<IFilteringHelper<BeerStyle, GetBeerStylesQuery>, BeerStylesFilteringHelper>();
        services.AddTransient<IFilteringHelper<Favorite, GetFavoritesQuery>, FavoritesFilteringHelper>();
        services.AddTransient<IFilteringHelper<Opinion, GetOpinionsQuery>, OpinionsFilteringHelper>();

        services.AddTransient<IBeersService, BeersService>();

        return services;
    }
}