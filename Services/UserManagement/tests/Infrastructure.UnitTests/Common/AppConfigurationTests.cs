using Infrastructure.Common;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.UnitTests.Common;

/// <summary>
///     Tests for the <see cref="AppConfiguration" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class AppConfigurationTests
{
    /// <summary>
    ///     The configuration.
    /// </summary>
    private IConfiguration? _configuration;

    /// <summary>
    ///     The app configuration.
    /// </summary>
    private AppConfiguration? _appConfiguration;

    /// <summary>
    ///     Tests that JwtSecret returns correct value.
    /// </summary>
    [Fact]
    public void JwtSecret_ShouldReturnCorrectValue()
    {
        // Arrange
        const string expectedJwtSecret = "test-secret";
        SetupConfiguration(new Dictionary<string, string?>
        {
            { "JwtSettings:Secret", expectedJwtSecret }
        });

        // Act
        var jwtSecret = _appConfiguration!.JwtSecret;

        // Assert
        jwtSecret.Should().Be(expectedJwtSecret);
    }

    /// <summary>
    ///     Tests that JwtSecret throws InvalidOperationException when JwtSecret value is null.
    /// </summary>
    [Fact]
    public void JwtSecret_ShouldThrowInvalidOperationException_WhenJwtSecretValueIsNull()
    {
        // Arrange
        SetupConfiguration(new Dictionary<string, string?>
        {
            { "JwtSettings:Secret", null }
        });

        // Act
        var act = () => { _ = _appConfiguration!.JwtSecret; };

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("JWT token secret key does not exists.");
    }

    /// <summary>
    ///     Setups configuration.
    /// </summary>
    /// <param name="config">The config data</param>
    private void SetupConfiguration(Dictionary<string, string?> config)
    {
        _configuration = new ConfigurationBuilder().AddInMemoryCollection(config).Build();
        _appConfiguration = new AppConfiguration(_configuration);
    }
}