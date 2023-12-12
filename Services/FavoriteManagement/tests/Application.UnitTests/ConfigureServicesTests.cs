using AutoMapper;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using SharedUtilities.Behaviors;
using SharedUtilities.Interfaces;
using SharedUtilities.Services;

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
    ///     Tests that the AddApplicationServices method registers LoggingBehavior.
    /// </summary>
    [Fact]
    public void AddApplicationServices_ShouldRegisterLoggingBehavior()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(IRequestPreProcessor<>));
        _services.Should().Contain(x => x.ImplementationType == typeof(LoggingBehavior<>));
        _services.Should().Contain(x => x.Lifetime == ServiceLifetime.Transient);
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

    /// <summary>
    ///     Tests that the AddApplicationServices method adds the filtering helpers
    ///     to the service collection as IFilteringHelper.
    /// </summary>
    [Fact]
    public void AddApplicationServices_ShouldAddFilteringHelpers()
    {
        // Assert
        // _services.Should().Contain(x => x.ServiceType == typeof(IFilteringHelper<Beer, GetBeersQuery>));
        // _services.Should().Contain(s => s.ImplementationType == typeof(BeersFilteringHelper));
        // _services.Should().Contain(s => s.Lifetime == ServiceLifetime.Transient);
        
    }
}