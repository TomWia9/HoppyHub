using Infrastructure.Identity;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Persistence.Configurations;

/// <summary>
///     Tests for the <see cref="UserConfiguration" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UserConfigurationTests
{
    /// <summary>
    ///     Tests that UserConfiguration configures entity correctly.
    /// </summary>
    [Fact]
    public void OpinionConfiguration_ShouldConfigureEntityCorrectly()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();
        var configuration = new UserConfiguration();
        var builder = modelBuilder.Entity<ApplicationUser>();

        // Act
        configuration.Configure(builder);
        var model = modelBuilder.FinalizeModel();
        var entity = model.FindEntityType(typeof(ApplicationUser));

        // Assert
        entity.Should().NotBeNull();
        entity!.FindProperty(nameof(ApplicationUser.Created))!.IsNullable.Should().BeFalse();
    }
}