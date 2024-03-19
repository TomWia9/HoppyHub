using System.Reflection;
using Application.Interfaces;
using Application.Services;
using Azure.Storage.Blobs;
using FluentValidation;
using MassTransit;
using Serilog;
using SharedUtilities;
using SharedUtilities.Exceptions;
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
    /// <param name="configuration">The configuration</param>
    public static void AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IBlobStorageService, BlobStorageService>();
        services.AddSingleton(_ => CreateBlobContainerClient(configuration));

        services.AddTransient<IImagesService, ImagesService>();

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(SharedUtilitiesAssemblyMarker)));
        services.AddMassTransit(x =>
        {
            x.AddConsumers(Assembly.GetExecutingAssembly());

            if (Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") == Environments.Development)
            {
                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration.GetValue<string>("RabbitMQ:Host"), "/", h =>
                    {
                        h.Username(configuration.GetValue<string>("RabbitMQ:Username"));
                        h.Password(configuration.GetValue<string>("RabbitMQ:Password"));
                    });

                    cfg.ConfigureEndpoints(context,
                        endpointNameFormatter: new DefaultEndpointNameFormatter(prefix: "ImageManagement"));
                    cfg.UseConsumeFilter(typeof(MessageValidationFilter<>), context);
                });
            }
            else
            {
                x.UsingAzureServiceBus((context, cfg) =>
                {
                    cfg.Host(configuration.GetValue<Uri>("AzureServiceBus:ConnectionString"));
                    cfg.ConfigureEndpoints(context,
                        endpointNameFormatter: new DefaultEndpointNameFormatter(prefix: "ImageManagement"));
                    cfg.UseConsumeFilter(typeof(MessageValidationFilter<>), context);
                });
            }
        });
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