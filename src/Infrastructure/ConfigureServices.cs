using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
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
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<ApplicationDbContextInitialiser>();

        services.AddTransient<IDateTime, DateTimeService>();
        
        return services;
    }
}