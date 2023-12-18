using Domain.Entities;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Persistence.Configurations;

/// <summary>
///     Tests for the <see cref="AddressConfiguration" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class AddressConfigurationTests
{
    /// <summary>
    ///     Tests that AddressConfiguration configures entity correctly.
    /// </summary>
    [Fact]
    public void AddressConfiguration_ShouldConfigureEntityCorrectly()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();
        var configuration = new AddressConfiguration();
        var builder = modelBuilder.Entity<Address>();

        // Act
        configuration.Configure(builder);
        var model = modelBuilder.FinalizeModel();
        var entity = model.FindEntityType(typeof(Address));

        // Assert
        entity.Should().NotBeNull();
        entity!.FindProperty(nameof(Address.Street))!.IsNullable.Should().BeFalse();
        entity.FindProperty(nameof(Address.Street))!.GetMaxLength().Should().Be(200);

        entity.FindProperty(nameof(Address.Number))!.IsNullable.Should().BeFalse();
        entity.FindProperty(nameof(Address.Number))!.GetMaxLength().Should().Be(10);

        entity.FindProperty(nameof(Address.PostCode))!.IsNullable.Should().BeFalse();
        entity.FindProperty(nameof(Address.PostCode))!.GetMaxLength().Should().Be(8);

        entity.FindProperty(nameof(Address.City))!.IsNullable.Should().BeFalse();
        entity.FindProperty(nameof(Address.City))!.GetMaxLength().Should().Be(50);

        entity.FindProperty(nameof(Address.State))!.IsNullable.Should().BeFalse();
        entity.FindProperty(nameof(Address.State))!.GetMaxLength().Should().Be(50);

        entity.FindProperty(nameof(Address.Country))!.IsNullable.Should().BeFalse();
        entity.FindProperty(nameof(Address.Country))!.GetMaxLength().Should().Be(50);

        entity.FindProperty(nameof(Address.BreweryId))!.IsNullable.Should().BeFalse();
    }
}