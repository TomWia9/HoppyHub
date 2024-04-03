using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharedUtilities.Interfaces;
using SharedUtilities.Services;

namespace Api.UnitTests;

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
        IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(new[]
        {
            new KeyValuePair<string, string>("UIAppUrl", "https://test.com"),
        }!).Build();
        
        _services = new ServiceCollection();
        _services.AddApiServices(configuration);
    }

    /// <summary>
    ///     Tests that the AddApiServices method registers the CurrentUserService as a scoped service.
    /// </summary>
    [Fact]
    public void AddApiServices_ShouldRegisterCurrentUserService()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(ICurrentUserService) &&
                                        x.ImplementationType == typeof(CurrentUserService) &&
                                        x.Lifetime == ServiceLifetime.Scoped);
    }

    /// <summary>
    ///     Tests that the AddApiServices method registers the HttpContextAccessor as a singleton service.
    /// </summary>
    [Fact]
    public void AddApiServices_ShouldRegisterHttpContextAccessor()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(IHttpContextAccessor) &&
                                        x.ImplementationType == typeof(HttpContextAccessor) &&
                                        x.Lifetime == ServiceLifetime.Singleton);
    }
    
    /// <summary>
    ///     Tests that the AddApiServices method adds cors.
    /// </summary>
    [Fact]
    public void AddApiServices_ShouldAddCors()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(ICorsService));

        var serviceProvider = _services.BuildServiceProvider();
        var corsOptions = serviceProvider.GetRequiredService<IOptions<CorsOptions>>().Value;
        var angularAppPolicy = corsOptions.GetPolicy("UIApp");
        
        angularAppPolicy.Should().NotBeNull();
        angularAppPolicy!.Origins.Count.Should().Be(1);
        angularAppPolicy.AllowAnyHeader.Should().BeTrue();
        angularAppPolicy.AllowAnyMethod.Should().BeTrue();
        angularAppPolicy.ExposedHeaders.Should().Contain("X-Pagination");
        angularAppPolicy.ExposedHeaders.Count.Should().Be(1);       
    }
}