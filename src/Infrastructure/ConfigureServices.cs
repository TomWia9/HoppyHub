using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

/// <summary>
///     The ConfigureServices class
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    ///     Adds infrastructure project services
    /// </summary>
    /// <param name="services">The services</param>
    /// <param name="configuration">The configuration</param>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        return services;
    }
}