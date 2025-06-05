using App.Interfaces;
using App.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace App.UnitTests;

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
        Mock<IWebHostEnvironment> webHostEnvironmentMock = new();

        _services = new ServiceCollection();
        _services.AddServices(configuration, webHostEnvironmentMock.Object);
    }

    /// <summary>
    ///     Tests that the AddServices method adds the EmailSender
    ///     to the service collection as IEmailSender.
    /// </summary>
    [Fact]
    public void AddApplicationServices_ShouldAddEmailSender()
    {
        // Assert
        _services.Should().Contain(x => x.ServiceType == typeof(IEmailSender));
        _services.Should().Contain(s => s.ImplementationType == typeof(EmailSender));
        _services.Should().Contain(s => s.Lifetime == ServiceLifetime.Singleton);
    }
}