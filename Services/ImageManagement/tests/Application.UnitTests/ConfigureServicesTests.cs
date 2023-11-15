using Application.Interfaces;
using Application.Services;
using Azure.Storage.Blobs;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.UnitTests;

/// <summary>
///     Unit tests for the <see cref="ConfigureServices" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class ConfigureServicesTests
{
    /// <summary>
    ///     The services.
    /// </summary>
    private readonly IServiceCollection _services;

    /// <summary>
    ///     Setups ConfigureServicesTests.
    /// </summary>
    public ConfigureServicesTests()
    {
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        _services = new ServiceCollection();
        _services.AddApplicationServices(configuration);
    }

    /// <summary>
    ///     Tests that the AddApplicationServices method adds the BlobStorageService
    ///     to the service collection as IBlobStorageService.
    /// </summary>
    [Fact]
    public void AddApplicationServices_ShouldAddBlobStorageService()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(IBlobStorageService));
        _services.Should().Contain(s => s.ImplementationType == typeof(BlobStorageService));
        _services.Should().Contain(s => s.Lifetime == ServiceLifetime.Singleton);
    }

    /// <summary>
    ///     Tests that the AddApplicationServices method adds the ImagesService
    ///     to the service collection as IImagesService.
    /// </summary>
    [Fact]
    public void AddApplicationServices_ShouldAddBeersImagesService()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(IImagesService));
        _services.Should().Contain(s => s.ImplementationType == typeof(ImagesService));
        _services.Should().Contain(s => s.Lifetime == ServiceLifetime.Transient);
    }

    /// <summary>
    ///     Tests that the AddApplicationServices method adds the BusControl
    ///     to the service collection as IBusControl.
    /// </summary>
    [Fact]
    public void AddApplicationServices_ShouldAddConsumersFromAssembly()
    {        
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(IBusControl));
    }
}