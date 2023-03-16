using Application.Common.Interfaces;
using Application.Common.Services;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Application.UnitTests;

/// <summary>
///     Unit tests for the <see cref="ConfigureServices"/> class.
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
        _services = new ServiceCollection();
        _services.AddApplicationServices();
    }

    /// <summary>
    ///     Tests that the AddApplicationServices method registers MediatR services
    ///     from the executing assembly to the service collection.
    /// </summary>
    [Fact]
    public void AddApplicationServices_ShouldRegisterMediatRServicesFromExecutingAssembly()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(IMediator));
    }
    
    /// <summary>
    ///     Tests that the AddApplicationServices method registers AutoMapper services
    ///     from the executing assembly to the service collection.
    /// </summary>
    [Fact]
    public void AddApplicationServices_ShouldRegisterAutoMapperFromExecutingAssembly()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(IMapper));
    }
    
    /// <summary>
    ///     Tests that the AddApplicationServices method adds the QueryService
    ///     to the service collection as IQueryService.
    /// </summary>
    [Fact]
    public void AddApplicationServices_ShouldAddQueryService()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(IQueryService<>));
        _services.Should().Contain(s => s.ImplementationType == typeof(QueryService<>));
        _services.Should().Contain(s => s.Lifetime == ServiceLifetime.Transient);
    }
}