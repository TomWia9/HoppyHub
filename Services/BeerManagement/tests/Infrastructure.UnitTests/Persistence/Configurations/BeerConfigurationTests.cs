using Domain.Entities;
using Infrastructure.Converters;
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

        entity.FindProperty(nameof(Beer.BeerStyleId))!.IsNullable.Should().BeFalse();

        entity.FindProperty(nameof(Beer.AlcoholByVolume))!.IsNullable.Should().BeFalse();

        entity.FindProperty(nameof(Beer.Description))!.IsNullable.Should().BeTrue();
        entity.FindProperty(nameof(Beer.Description))!.GetMaxLength().Should().Be(3000);

        entity.FindProperty(nameof(Beer.Composition))!.IsNullable.Should().BeTrue();
        entity.FindProperty(nameof(Beer.Composition))!.GetMaxLength().Should().Be(300);

        var releaseDateProperty = entity.FindProperty(nameof(Beer.ReleaseDate))!;
        //releaseDateProperty!.IsNullable.Should().BeFalse(); //TODO: Fix nullable
        releaseDateProperty.GetValueConverter().Should().BeOfType<DateOnlyConverter>();

        entity.FindProperty(nameof(Beer.BreweryId))!.IsNullable.Should().BeFalse();

        entity.FindProperty(nameof(Beer.Rating))!.IsNullable.Should().BeFalse();
        entity.FindProperty(nameof(Beer.Rating))!.GetDefaultValue().Should().Be(0);

        entity.FindProperty(nameof(Beer.OpinionsCount))!.IsNullable.Should().BeFalse();
        entity.FindProperty(nameof(Beer.OpinionsCount))!.GetDefaultValue().Should().Be(0);

        entity.FindProperty(nameof(Beer.FavoritesCount))!.IsNullable.Should().BeFalse();
        entity.FindProperty(nameof(Beer.FavoritesCount))!.GetDefaultValue().Should().Be(0);

        var beerImageNavigation = entity.FindNavigation(nameof(Beer.BeerImage));
        beerImageNavigation!.IsCollection.Should().BeFalse();
        beerImageNavigation.ForeignKey.PrincipalEntityType.ClrType.Should().Be(typeof(Beer));
    }
}