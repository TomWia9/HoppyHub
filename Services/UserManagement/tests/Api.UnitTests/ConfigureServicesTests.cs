﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SharedUtilities.Interfaces;
using SharedUtilities.Services.Services;

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
        _services = new ServiceCollection();
        _services.AddApiServices();
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
}