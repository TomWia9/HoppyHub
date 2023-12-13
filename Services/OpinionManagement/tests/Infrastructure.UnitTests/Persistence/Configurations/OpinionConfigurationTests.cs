using Domain.Entities;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Persistence.Configurations;

/// <summary>
///     Tests for the <see cref="OpinionConfiguration" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class OpinionConfigurationTests
{
    /// <summary>
    ///     Tests that OpinionConfiguration configures entity correctly.
    /// </summary>
    [Fact]
    public void OpinionConfiguration_ShouldConfigureEntityCorrectly()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();
        var configuration = new OpinionConfiguration();
        var builder = modelBuilder.Entity<Opinion>();

        // Act
        configuration.Configure(builder);
        var model = modelBuilder.FinalizeModel();
        var entity = model.FindEntityType(typeof(Opinion));

        // Assert
        entity.Should().NotBeNull();
        entity!.FindProperty(nameof(Opinion.Rating))!.IsNullable.Should().BeFalse();

        entity.FindProperty(nameof(Opinion.Comment))!.IsNullable.Should().BeTrue();
        entity.FindProperty(nameof(Opinion.Comment))!.GetMaxLength().Should().Be(1000);

        entity.FindProperty(nameof(Opinion.ImageUri))!.IsNullable.Should().BeTrue();
        entity.FindProperty(nameof(Opinion.ImageUri))!.GetMaxLength().Should().Be(200);

        entity.FindProperty(nameof(Opinion.CreatedBy))!.IsNullable.Should().BeFalse();

        entity.FindProperty(nameof(Opinion.Created))!.IsNullable.Should().BeFalse();

        entity.FindProperty(nameof(Opinion.LastModifiedBy))!.IsNullable.Should().BeFalse();

        entity.FindProperty(nameof(Opinion.LastModified))!.IsNullable.Should().BeFalse();
    }
}