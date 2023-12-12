using Domain.Common;
using Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.UnitTests.Persistence.Configurations;

/// <summary>
///     Tests for the <see cref="BaseConfiguration{TEntity}" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BaseConfigurationTests
{
    /// <summary>
    ///     Test entity.
    /// </summary>
    private class TestEntity : BaseAuditableEntity;

    /// <summary>
    ///     Test entity configuration.
    /// </summary>
    private class TestEntityConfiguration : BaseConfiguration<TestEntity>;

    /// <summary>
    ///     Tests that BaseConfiguration configures entity correctly.
    /// </summary>
    [Fact]
    public void BaseConfiguration_ShouldConfigureEntityCorrectly()
    {
        // Arrange
        var modelBuilder = new ModelBuilder();
        var configuration = new TestEntityConfiguration();
        var builder = modelBuilder.Entity<TestEntity>();

        // Act
        configuration.Configure(builder);
        var model = modelBuilder.FinalizeModel();
        var entity = model.FindEntityType(typeof(TestEntity));

        // Assert
        entity.Should().NotBeNull();
        entity!.FindPrimaryKey().Should().NotBeNull();
        entity.FindProperty(nameof(BaseAuditableEntity.Created))!.GetMaxLength().Should().Be(50);
        entity.FindProperty(nameof(BaseAuditableEntity.CreatedBy))!.GetMaxLength().Should().Be(40);
        entity.FindProperty(nameof(BaseAuditableEntity.LastModified))!.GetMaxLength().Should().Be(50);
        entity.FindProperty(nameof(BaseAuditableEntity.LastModifiedBy))!.GetMaxLength().Should().Be(40);
    }
}