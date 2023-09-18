using Application.Common.Mappings;

namespace Application.UnitTests.Common.Mappings;

/// <summary>
///     Unit tests for the <see cref="MappingExtensions" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class MappingExtensionsTests
{
    /// <summary>
    ///     Tests that ToPaginatedList method creates paginated list.
    /// </summary>
    [Fact]
    public void ToPaginatedList_ShouldCreatePaginatedList()
    {
        // Arrange
        const int pageNumber = 2;
        const int pageSize = 5;
        const int count = 10;
        var expectedTotalPages = (int)Math.Ceiling(count / (double)pageSize);
        var source = Enumerable.Range(1, count).Select(x => new TestObject(x)).ToList();

        // Act
        var result = source.ToPaginatedList(pageNumber, pageSize);

        // Assert
        result.Should().NotBeNull();
        result.CurrentPage.Should().Be(pageNumber);
        result.PageSize.Should().Be(pageSize);
        result.TotalCount.Should().Be(source.Count);
        result.TotalPages.Should().Be(expectedTotalPages);
        result.HasPrevious.Should().Be(true, "current page number is set to 2");
        result.HasNext.Should().Be(pageNumber < expectedTotalPages);
        result[0].Should().BeEquivalentTo(new TestObject(6));
        result[4].Should().BeEquivalentTo(new TestObject(10));
    }

    /// <summary>
    ///     TestObject record.
    /// </summary>
    private record TestObject(int Id);
}