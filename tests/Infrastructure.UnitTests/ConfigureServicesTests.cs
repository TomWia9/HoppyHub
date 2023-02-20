using System.Diagnostics.CodeAnalysis;
using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.UnitTests;

/// <summary>
///     Tests for the <see cref="ConfigureServices"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class ConfigureServicesTests
{
    /// <summary>
    ///     The services
    /// </summary>
    private readonly IServiceCollection _services;

    /// <summary>
    ///     Setups tests
    /// </summary>
    public ConfigureServicesTests()
    {
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        _services = new ServiceCollection();
        _services.AddInfrastructureServices(configuration);
    }

    /// <summary>
    ///     Tests that the AddInfrastructureServices method adds the ApplicationDbContext to the service collection
    /// </summary>
    [Fact]
    public void AddInfrastructureServices_AddsDbContext()
    {
        // Assert
        Assert.Contains(_services, service => service.ServiceType == typeof(ApplicationDbContext));
    }

    /// <summary>
    ///     Tests that the AddInfrastructureServices method adds the AuditableEntitySaveChangesInterceptor to the service collection
    /// </summary>
    [Fact]
    public void AddInfrastructureServices_AddsAuditableEntitySaveChangesInterceptor()
    {
        // Assert
        Assert.Contains(_services, service => service.ServiceType == typeof(AuditableEntitySaveChangesInterceptor));
    }

    /// <summary>
    ///     Tests that the AddInfrastructureServices method adds the IApplicationDbContext to the service collection
    /// </summary>
    [Fact]
    public void AddInfrastructureServices_AddsIApplicationDbContext()
    {
        // Assert
        Assert.Contains(_services, service => service.ServiceType == typeof(IApplicationDbContext));
    }

    /// <summary>
    ///     Tests that the AddInfrastructureServices method adds the ApplicationDbContextInitialiser to the service collection
    /// </summary>
    [Fact]
    public void AddInfrastructureServices_AddsApplicationDbContextInitialiser()
    {
        // Assert
        Assert.Contains(_services, service => service.ServiceType == typeof(ApplicationDbContextInitialiser));
    }

    /// <summary>
    ///     Tests that the AddInfrastructureServices method adds the DateTimeService to the service collection as IDateTime
    /// </summary>
    [Fact]
    public void AddInfrastructureServices_AddsDateTimeService()
    {
        // Assert
        Assert.Contains(_services, service => service.ServiceType == typeof(IDateTime));
        Assert.Contains(_services, service => service.ImplementationType == typeof(DateTimeService));
    }
}