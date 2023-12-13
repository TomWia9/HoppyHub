using Domain.Common;
using Domain.Entities;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Persistence.Configurations;

/// <summary>
///     Tests for the <see cref="BreweryConfiguration" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BreweryConfigurationTests
{
    /// <summary>
    ///     Tests that BreweryConfiguration configures entity correctly.
    /// </summary>
    [Fact]
    public void BreweryConfiguration_ShouldConfigureEntityCorrectly()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();
        var configuration = new BreweryConfiguration();
        var builder = modelBuilder.Entity<Brewery>();

        // Act
        configuration.Configure(builder);
        var model = modelBuilder.FinalizeModel();
        var entity = model.FindEntityType(typeof(Brewery));

        // Assert
        entity.Should().NotBeNull();
        entity!.FindProperty(nameof(Brewery.Name))!.IsNullable.Should().BeFalse();
        entity.FindProperty(nameof(Brewery.Name))!.GetMaxLength().Should().Be(500);

        entity.FindProperty(nameof(Brewery.Description))!.IsNullable.Should().BeFalse();
        entity.FindProperty(nameof(Brewery.Description))!.GetMaxLength().Should().Be(5000);

        entity.FindProperty(nameof(Brewery.FoundationYear))!.IsNullable.Should().BeFalse();

        entity.FindProperty(nameof(Brewery.WebsiteUrl))!.IsNullable.Should().BeTrue();
        entity.FindProperty(nameof(Brewery.WebsiteUrl))!.GetMaxLength().Should().Be(200);

        var addressNavigation = entity.FindNavigation(nameof(Brewery.Address));
        addressNavigation!.IsCollection.Should().BeFalse();
        addressNavigation.ForeignKey.PrincipalEntityType.ClrType.Should().Be(typeof(Brewery));

        var beersNavigation = entity.FindNavigation(nameof(Brewery.Beers))!;
        beersNavigation.IsCollection.Should().BeTrue();
        beersNavigation.ForeignKey.PrincipalEntityType.ClrType.Should().Be(typeof(Brewery));
        beersNavigation.ForeignKey.IsRequired.Should().BeTrue();

        entity.FindProperty(nameof(BaseAuditableEntity.Created))!.GetMaxLength().Should().Be(50);
        entity.FindProperty(nameof(BaseAuditableEntity.CreatedBy))!.GetMaxLength().Should().Be(40);
        entity.FindProperty(nameof(BaseAuditableEntity.LastModified))!.GetMaxLength().Should().Be(50);
        entity.FindProperty(nameof(BaseAuditableEntity.LastModifiedBy))!.GetMaxLength().Should().Be(40);
    }
}