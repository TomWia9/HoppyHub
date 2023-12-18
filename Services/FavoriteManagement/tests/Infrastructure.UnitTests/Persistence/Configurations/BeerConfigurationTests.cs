using Domain.Entities;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Persistence.Configurations;

/// <summary>
///     Tests for the <see cref="BeerConfiguration" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeerConfigurationTests
{
    /// <summary>
    ///     Tests that BeerConfiguration configures entity correctly.
    /// </summary>
    [Fact]
    public void BeerConfiguration_ShouldConfigureEntityCorrectly()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();
        var configuration = new BeerConfiguration();
        var builder = modelBuilder.Entity<Beer>();

        // Act
        configuration.Configure(builder);
        var model = modelBuilder.FinalizeModel();
        var entity = model.FindEntityType(typeof(Beer));

        // Assert
        entity.Should().NotBeNull();
        entity!.FindProperty(nameof(Beer.Name))!.IsNullable.Should().BeFalse();
        entity.FindProperty(nameof(Beer.Name))!.GetMaxLength().Should().Be(200);

        entity.FindProperty(nameof(Beer.BreweryName))!.IsNullable.Should().BeFalse();
        entity.FindProperty(nameof(Beer.BreweryName))!.GetMaxLength().Should().Be(500);

        var opinionsNavigation = entity.FindNavigation(nameof(Beer.Favorites))!;
        opinionsNavigation.IsCollection.Should().BeTrue();
        opinionsNavigation.ForeignKey.PrincipalEntityType.ClrType.Should().Be(typeof(Beer));
        opinionsNavigation.ForeignKey.IsRequired.Should().BeTrue();
    }
}