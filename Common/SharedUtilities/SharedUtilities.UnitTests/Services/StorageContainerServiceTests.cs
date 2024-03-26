using SharedUtilities.Services;

namespace SharedUtilities.UnitTests.Services;

/// <summary>
///     Tests for the <see cref="StorageContainerService" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class StorageContainerServiceTests
{
    private const string StorageAccountConnectionString = nameof(StorageAccountConnectionString);
    private const string ContainerName = nameof(ContainerName);

    /// <summary>
    ///     Tests that StorageContainerService constructor throws ArgumentNullException when container name is null or empty.
    /// </summary>
    /// <param name="containerName">The container name</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void StorageContainerServiceConstructor_ShouldThrowArgumentNullException_WhenContainerNameIsNullOrEmpty(
        string? containerName)
    {
        // Act
        var act = () => { _ = new StorageContainerService(StorageAccountConnectionString, containerName); };

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    /// <summary>
    ///     Tests that StorageContainerService constructor throws ArgumentNullException when storage account connection string is null or empty.
    /// </summary>
    /// <param name="storageAccountConnectionString">The storage account connection string</param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void
        StorageContainerServiceConstructor_ShouldThrowArgumentNullException_WhenStorageAccountConnectionStringIsNullOrEmpty(
            string? storageAccountConnectionString)
    {
        // Act
        var act = () => { _ = new StorageContainerService(storageAccountConnectionString, ContainerName); };

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }
}