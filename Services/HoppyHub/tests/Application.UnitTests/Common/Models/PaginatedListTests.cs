using Application.Common.Models;

namespace Application.UnitTests.Common.Models;

/// <summary>
///     Unit tests for the <see cref="PaginatedList{T}" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class PaginatedListTests
{
    /// <summary>
    ///     Tests that Create method returns correct paginated list.
    /// </summary>
    [Fact]
    public void Create_ShouldReturnPaginatedList()
    {
        // Arrange
        var list = Enumerable.Range(1, 10).ToList();

        // Act
        var result = PaginatedList<int>.Create(list, 2, 3);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeAssignableTo<PaginatedList<int>>();
        result.Should().HaveCount(3);
        result.CurrentPage.Should().Be(2);
        result.PageSize.Should().Be(3);
        result.TotalCount.Should().Be(10);
        result.TotalPages.Should().Be(4);
        result.HasPrevious.Should().BeTrue();
        result.HasNext.Should().BeTrue();
    }

    /// <summary>
    ///     Tests that GetMetadata method returns Json metadata.
    /// </summary>
    [Fact]
    public void GetMetadata_ShouldReturnJsonMetadata()
    {
        var source = Enumerable.Range(1, 10).ToList();

        // Arrange
        var paginatedList = PaginatedList<int>.Create(source, 2, 3);

        // Act
        var result = paginatedList.GetMetadata();

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should()
            .Be(
                "{\"TotalCount\":10,\"PageSize\":3,\"CurrentPage\":2,\"TotalPages\":4,\"HasNext\":true,\"HasPrevious\":true}");
    }
}