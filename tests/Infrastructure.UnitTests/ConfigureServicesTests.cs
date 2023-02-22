using Application.Common.Interfaces;
using Infrastructure.Identity;
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
    ///     The services.
    /// </summary>
    private readonly IServiceCollection _services;

    /// <summary>
    ///     Setups tests.
    /// </summary>
    public ConfigureServicesTests()
    {
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection().Build();
        _services = new ServiceCollection();
        _services.AddInfrastructureServices(configuration);
    }

    /// <summary>
    ///     Tests that the AddInfrastructureServices method adds the ApplicationDbContext to the service collection.
    /// </summary>
    [Fact]
    public void AddInfrastructureServices_AddsDbContext()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(ApplicationDbContext));
        _services.Should().Contain(s => s.ImplementationType == typeof(ApplicationDbContext));
        _services.Should().Contain(s => s.Lifetime == ServiceLifetime.Scoped);
    }

    /// <summary>
    ///     Tests that the AddInfrastructureServices method adds the AuditableEntitySaveChangesInterceptor
    ///     to the service collection.
    /// </summary>
    [Fact]
    public void AddInfrastructureServices_AddsAuditableEntitySaveChangesInterceptor()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(AuditableEntitySaveChangesInterceptor));
        _services.Should().Contain(s => s.ImplementationType == typeof(AuditableEntitySaveChangesInterceptor));
        _services.Should().Contain(s => s.Lifetime == ServiceLifetime.Scoped);
    }

    /// <summary>
    ///     Tests that the AddInfrastructureServices method adds the IApplicationDbContext to the service collection.
    /// </summary>
    [Fact]
    public void AddInfrastructureServices_AddsIApplicationDbContext()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(IApplicationDbContext));
        _services.Should().Contain(s => s.ImplementationType == typeof(ApplicationDbContext));
        _services.Should().Contain(s => s.Lifetime == ServiceLifetime.Scoped);
    }

    /// <summary>
    ///     Tests that the AddInfrastructureServices method adds the ApplicationDbContextInitialiser
    ///     to the service collection.
    /// </summary>
    [Fact]
    public void AddInfrastructureServices_AddsApplicationDbContextInitialiser()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(ApplicationDbContextInitialiser));
        _services.Should().Contain(s => s.ImplementationType == typeof(ApplicationDbContextInitialiser));
        _services.Should().Contain(s => s.Lifetime == ServiceLifetime.Scoped);
    }

    /// <summary>
    ///     Tests that the AddInfrastructureServices method adds the DateTimeService
    ///     to the service collection as IDateTime.
    /// </summary>
    [Fact]
    public void AddInfrastructureServices_AddsDateTimeService()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(IDateTime));
        _services.Should().Contain(s => s.ImplementationType == typeof(DateTimeService));
        _services.Should().Contain(s => s.Lifetime == ServiceLifetime.Transient);
    }
    
    /// <summary>
    ///     Tests that the AddInfrastructureServices method adds the IdentityService
    ///     to the service collection as IIdentityService.
    /// </summary>
    [Fact]
    public void AddInfrastructureServices_AddsIdentityService()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(IIdentityService));
        _services.Should().Contain(s => s.ImplementationType == typeof(IdentityService));
        _services.Should().Contain(s => s.Lifetime == ServiceLifetime.Transient);
    }
    
    /// <summary>
    ///     Tests that the AddInfrastructureServices method adds the JwtSettings
    ///     to the service collection as JwtSettings.
    /// </summary>
    [Fact]
    public void AddInfrastructureServices_AddsJwtSettings()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(JwtSettings));
        _services.Should().Contain(x => x.Lifetime == ServiceLifetime.Singleton);
    }
}