using System.Reflection;
using System.Text;
using Application;
using Application.Common.Interfaces;
using Infrastructure.Common;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using SharedUtilities.Filters;
using SharedUtilities.Models;

namespace Infrastructure;

/// <summary>
///     The ConfigureServices class.
/// </summary>
public static class ConfigureServices
{
    /// <summary>
    ///     Adds infrastructure project services.
    /// </summary>
    /// <param name="services">The services</param>
    /// <param name="configuration">The configuration</param>
    public static void AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddMassTransit(x =>
        {
            x.AddConsumers(Assembly.GetAssembly(typeof(IAssemblyMarker)));

            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development)
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration.GetValue<string>("RabbitMQ:Host"), "/", h =>
                    {
                        h.Username(configuration.GetValue<string>("RabbitMQ:Username"));
                        h.Password(configuration.GetValue<string>("RabbitMQ:Password"));
                    });

                    cfg.ConfigureEndpoints(context,
                        endpointNameFormatter: new DefaultEndpointNameFormatter(prefix: "BeerManagement"));
                    cfg.UseConsumeFilter(typeof(MessageValidationFilter<>), context);
                });
            }
            else
            {
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(configuration.GetValue<string>("AzureServiceBus:ConnectionString"));
                    cfg.ConfigureEndpoints(context,
                        endpointNameFormatter: new DefaultEndpointNameFormatter(prefix: "BeerManagement"));
                    cfg.UseConsumeFilter(typeof(MessageValidationFilter<>), context);
                });
            }
        });

        services.AddSingleton<IAppConfiguration, AppConfiguration>();
        services.AddSingleton(TimeProvider.System);

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IApplicationDbContextInitializer, ApplicationDbContextInitializer>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(jwtOptions =>
            {
                jwtOptions.SaveToken = true;
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.ASCII.GetBytes(
                                configuration.GetValue<string>("JwtSettings:Secret") ??
                                throw new InvalidOperationException("JWT token secret key does not exists."))),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.UserAccess, policy => policy.RequireAssertion(context =>
                context.User.IsInRole(Roles.User) || context.User.IsInRole(Roles.Administrator)))
            .AddPolicy(Policies.AdministratorAccess,
                policy => policy.RequireAssertion(context => context.User.IsInRole(Roles.Administrator)));
    }
}