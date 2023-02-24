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
    ///     Setups tests.
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
}