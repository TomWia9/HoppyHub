using Domain.Entities;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Persistence.Configurations;

/// <summary>
///     Tests for the <see cref="BeerStyleConfiguration" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BeerStyleConfigurationTests
{
    /// <summary>
    ///     Tests that BeerStyleConfiguration configures entity correctly.
    /// </summary>
    [Fact]
    public void BeerStyleConfiguration_ShouldConfigureEntityCorrectly()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();
        var configuration = new BeerStyleConfiguration();
        var builder = modelBuilder.Entity<BeerStyle>();

        // Act
        configuration.Configure(builder);
        var model = modelBuilder.FinalizeModel();
        var entity = model.FindEntityType(typeof(BeerStyle));

        // Assert
        entity.Should().NotBeNull();
        entity!.FindProperty(nameof(BeerStyle.Name))!.IsNullable.Should().BeFalse();
        entity.FindProperty(nameof(BeerStyle.Name))!.GetMaxLength().Should().Be(100);

        entity.FindProperty(nameof(BeerStyle.Description))!.IsNullable.Should().BeFalse();
        entity.FindProperty(nameof(BeerStyle.Description))!.GetMaxLength().Should().Be(1000);

        entity.FindProperty(nameof(BeerStyle.CountryOfOrigin))!.IsNullable.Should().BeFalse();
        entity.FindProperty(nameof(BeerStyle.CountryOfOrigin))!.GetMaxLength().Should().Be(50);

        var beersNavigation = entity.FindNavigation(nameof(BeerStyle.Beers))!;
        beersNavigation.IsCollection.Should().BeTrue();
        beersNavigation.ForeignKey.PrincipalEntityType.ClrType.Should().Be(typeof(BeerStyle));
        beersNavigation.ForeignKey.IsRequired.Should().BeTrue();
    }
}