﻿using System.Text;
using Application.Common.Interfaces;
using Infrastructure.Common;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Services;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
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
            options.UseSqlServer(configuration.GetConnectionString("UserManagementDbConnection"),
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddMassTransit(x =>
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == Environments.Development)
            {
                x.UsingRabbitMq((_, cfg) =>
                {
                    cfg.Host(configuration.GetValue<string>("RabbitMQ:Host"), "/", h =>
                    {
                        h.Username(configuration.GetValue<string>("RabbitMQ:Username") ?? "");
                        h.Password(configuration.GetValue<string>("RabbitMQ:Password") ?? "");
                    });
                });
            }
            else
            {
                x.UsingAzureServiceBus((_, cfg) =>
                {
                    cfg.Host(configuration.GetConnectionString("AzureServiceBusConnection"));
                });
            }
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IApplicationDbContextInitializer, ApplicationDbContextInitializer>();

        services.AddTransient<IIdentityService, IdentityService>();
        services.AddTransient<IUsersService, UsersService>();

        services.AddSingleton<IAppConfiguration, AppConfiguration>();

        services.AddIdentityCore<ApplicationUser>()
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

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

        services.Configure<IdentityOptions>(options => { options.User.RequireUniqueEmail = true; });
    }
}