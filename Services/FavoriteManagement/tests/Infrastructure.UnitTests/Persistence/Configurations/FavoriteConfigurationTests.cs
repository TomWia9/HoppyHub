using Domain.Entities;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Persistence.Configurations;

/// <summary>
///     Tests for the <see cref="FavoriteConfiguration" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class FavoriteConfigurationTests
{
    /// <summary>
    ///     Tests that FavoriteConfiguration configures entity correctly.
    /// </summary>
    [Fact]
    public void FavoriteConfiguration_ShouldConfigureEntityCorrectly()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();
        var configuration = new FavoriteConfiguration();
        var builder = modelBuilder.Entity<Favorite>();

        // Act
        configuration.Configure(builder);
        var model = modelBuilder.FinalizeModel();
        var entity = model.FindEntityType(typeof(Favorite));

        // Assert
        entity.Should().NotBeNull();
        entity!.FindProperty(nameof(Favorite.BeerId))!.IsNullable.Should().BeFalse();

        entity.FindProperty(nameof(Favorite.CreatedBy))!.IsNullable.Should().BeFalse();

        entity.FindProperty(nameof(Favorite.Created))!.IsNullable.Should().BeFalse();

        entity.FindProperty(nameof(Favorite.LastModifiedBy))!.IsNullable.Should().BeFalse();

        entity.FindProperty(nameof(Favorite.LastModified))!.IsNullable.Should().BeFalse();
    }
}