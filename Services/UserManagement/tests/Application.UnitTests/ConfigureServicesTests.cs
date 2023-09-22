using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using SharedUtilities.Behaviors;

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
}