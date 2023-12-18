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
    ///     The app configuration.
    /// </summary>
    private AppConfiguration? _appConfiguration;

    /// <summary>
    ///     The configuration.
    /// </summary>
    private IConfiguration? _configuration;

    /// <summary>
    ///     Tests that TempBeerImageUri returns correct value.
    /// </summary>
    [Fact]
    public void TempBeerImageUri_ShouldReturnCorrectValue()
    {
        // Arrange
        const string expectedTempBeerImageUri = "https://example.com/image.jpg";
        SetupConfiguration(new Dictionary<string, string?>
        {
            { "TempBeerImageUri", expectedTempBeerImageUri }
        });

        // Act
        var tempBeerImageUri = _appConfiguration!.TempBeerImageUri;

        // Assert
        tempBeerImageUri.Should().Be(expectedTempBeerImageUri);
    }

    /// <summary>
    ///     Tests that TempBeerImageUri throws InvalidOperationException when TempBeerImage value is null.
    /// </summary>
    [Fact]
    public void TempBeerImageUri_ShouldThrowInvalidOperationException_WhenTempBeerImageValueIsNull()
    {
        // Arrange
        SetupConfiguration(new Dictionary<string, string?>
        {
            { "TempBeerImageUri", null }
        });

        // Act
        var act = () => { _ = _appConfiguration!.TempBeerImageUri; };

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Temp beer image uri does not exists.");
    }

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