using System.Text;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Azure.Storage.Blobs;
using Infrastructure.AzureServices;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;

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
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                builder => builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IApplicationDbContextInitializer, ApplicationDbContextInitializer>();

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<IBeersImagesService, BeersImagesService>();
        services.AddTransient<IOpinionsImagesService, OpinionsImagesService>();
        services.AddSingleton<IAzureStorageService, AzureStorageService>();
        services.AddSingleton(_ => CreateBlobContainerClient(configuration));

        var jwtSettings = new JwtSettings();
        configuration.Bind(nameof(JwtSettings), jwtSettings);
        services.AddSingleton(jwtSettings);

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
                            Encoding.ASCII.GetBytes(jwtSettings.Secret ?? throw new InvalidOperationException())),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = true,
                    ValidateLifetime = true
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.UserAccess,
                policy => policy.RequireAssertion(context =>
                    context.User.IsInRole(Roles.User) || context.User.IsInRole(Roles.Administrator)));

            options.AddPolicy(Policies.AdministratorAccess,
                policy => policy.RequireAssertion(context => context.User.IsInRole(Roles.Administrator)));
        });
        
        return services;
    }

    /// <summary>
    ///     Creates blob container client.
    /// </summary>
    /// <param name="configuration">The configuration</param>
    private static BlobContainerClient CreateBlobContainerClient(IConfiguration configuration)
    {
        var blobConnectionString = configuration.GetValue<string>("BlobContainerSettings:BlobConnectionString");
        var blobContainerName = configuration.GetValue<string>("BlobContainerSettings:BlobContainerName");

        if (string.IsNullOrEmpty(blobConnectionString))
        {
            throw new RemoteServiceConnectionException("The blob storage connection string is null");
        }

        if (string.IsNullOrEmpty(blobContainerName))
        {
            throw new RemoteServiceConnectionException("The blob storage container name is null");
        }

        try
        {
            return new BlobContainerClient(blobConnectionString, blobContainerName);
        }
        catch (Exception e)
        {
            Log.Logger.Error("Cannot connect to te blob container. Exception message: {ExMessage}", e.Message);
            throw new RemoteServiceConnectionException(e.Message);
        }
    }
}