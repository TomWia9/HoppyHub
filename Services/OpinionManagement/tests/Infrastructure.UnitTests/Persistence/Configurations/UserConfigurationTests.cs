using Domain.Entities;
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
    public void UserConfiguration_ShouldConfigureEntityCorrectly()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();
        var configuration = new UserConfiguration();
        var builder = modelBuilder.Entity<User>();

        // Act
        configuration.Configure(builder);
        var model = modelBuilder.FinalizeModel();
        var entity = model.FindEntityType(typeof(User));

        // Assert
        entity.Should().NotBeNull();
        entity!.FindProperty(nameof(User.Username))!.IsNullable.Should().BeFalse();
        entity.FindProperty(nameof(User.Username))!.GetMaxLength().Should().Be(256);

        entity.FindProperty(nameof(User.Role))!.IsNullable.Should().BeFalse();
        entity.FindProperty(nameof(User.Role))!.GetMaxLength().Should().Be(15);

        entity.FindProperty(nameof(User.Deleted))!.IsNullable.Should().BeFalse();

        var opinionsNavigation = entity.FindNavigation(nameof(User.Opinions))!;
        opinionsNavigation.IsCollection.Should().BeTrue();
        opinionsNavigation.ForeignKey.PrincipalEntityType.ClrType.Should().Be(typeof(User));
        opinionsNavigation.ForeignKey.Properties.Should().Contain(x => x.Name == nameof(Opinion.CreatedBy));
    }
}