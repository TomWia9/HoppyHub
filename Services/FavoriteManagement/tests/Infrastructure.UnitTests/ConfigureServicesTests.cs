using Application.Common.Interfaces;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Interceptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.UnitTests;

/// <summary>
///     Tests for the <see cref="ConfigureServices" /> class.
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
        _services.AddInfrastructureServices(configuration);
    }

    /// <summary>
    ///     Tests that the AddInfrastructureServices method adds the ApplicationDbContext to the service collection.
    /// </summary>
    [Fact]
    public void AddInfrastructureServices_Should_AddDbContext()
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
    public void AddInfrastructureServices_ShouldAddAuditableEntitySaveChangesInterceptor()
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
    public void AddInfrastructureServices_ShouldAddIApplicationDbContext()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(IApplicationDbContext));
        _services.Should().Contain(s => s.ImplementationType == typeof(ApplicationDbContext));
        _services.Should().Contain(s => s.Lifetime == ServiceLifetime.Scoped);
    }

    /// <summary>
    ///     Tests that the AddInfrastructureServices method adds the IApplicationDbContextInitializer to the service
    ///     collection.
    /// </summary>
    [Fact]
    public void AddInfrastructureServices_Should_AddIApplicationDbContextInitializer()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(IApplicationDbContextInitializer));
        _services.Should().Contain(s => s.ImplementationType == typeof(ApplicationDbContextInitializer));
        _services.Should().Contain(s => s.Lifetime == ServiceLifetime.Scoped);
    }

    /// <summary>
    ///     Tests that the AddInfrastructureServices method adds the TimeProvider
    ///     to the service collection.
    /// </summary>
    [Fact]
    public void AddInfrastructureServices_ShouldAddTimeProvider()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(TimeProvider));
        _services.Should().Contain(s => s.Lifetime == ServiceLifetime.Transient);
    }

    /// <summary>
    ///     Tests that the AddInfrastructureServices method adds the JwtSettings
    ///     to the service collection as JwtSettings.
    /// </summary>
    [Fact]
    public void AddInfrastructureServices_ShouldAddJwtSettings()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(JwtSettings));
        _services.Should().Contain(x => x.Lifetime == ServiceLifetime.Singleton);
    }
}