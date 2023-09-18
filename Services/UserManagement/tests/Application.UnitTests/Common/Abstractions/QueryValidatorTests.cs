using Application.Common.Abstractions;
using Application.Common.Enums;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Common.Abstractions;

/// <summary>
///     Tests for the <see cref="QueryValidator{T}" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class QueryValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly QueryValidator<TestQueryParameters> _validator;

    /// <summary>
    ///     Setups QueryValidatorTests.
    /// </summary>
    public QueryValidatorTests()
    {
        _validator = new TestQueryValidator();
    }

    /// <summary>
    ///     Tests that validation should have error when page number is less than 1.
    /// </summary>
    [Fact]
    public void ShouldHaveError_WhenPageNumberIsLessThan1()
    {
        // Arrange
        var queryParameters = new TestQueryParameters { PageNumber = 0 };

        // Act
        var result = _validator.TestValidate(queryParameters);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageNumber);
    }

    /// <summary>
    ///     Tests that validation should not have error when page number is greater than or equal to 1.
    /// </summary>
    [Fact]
    public void ShouldNotHaveError_WhenPageNumberIsGreaterThanOrEqualTo1()
    {
        // Arrange
        var queryParameters = new TestQueryParameters { PageNumber = 1 };

        // Act
        var result = _validator.TestValidate(queryParameters);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageNumber);
    }

    /// <summary>
    ///     Tests that validation should have error when page size is less than 1.
    /// </summary>
    [Fact]
    public void ShouldHaveError_WhenPageSizeIsLessThan1()
    {
        // Arrange
        var queryParameters = new TestQueryParameters { PageSize = 0 };

        // Act
        var result = _validator.TestValidate(queryParameters);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    /// <summary>
    ///     Tests that validation should not have error when page size is greater than or equal to 1.
    /// </summary>
    [Fact]
    public void ShouldNotHaveError_WhenPageSizeIsGreaterThanOrEqualTo1()
    {
        // Arrange
        var queryParameters = new TestQueryParameters { PageSize = 1 };

        // Act
        var result = _validator.TestValidate(queryParameters);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.PageSize);
    }

    /// <summary>
    ///     Tests that validation should have error when search query is greater than 100.
    /// </summary>
    [Fact]
    public void ShouldHaveError_WhenSearchQueryLengthIsGreaterThan100()
    {
        // Arrange
        var queryParameters = new TestQueryParameters { SearchQuery = new string('a', 101) };

        // Act
        var result = _validator.TestValidate(queryParameters);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SearchQuery);
    }

    /// <summary>
    ///     Tests that validation should not have error when search query is less than or equal to 100.
    /// </summary>
    [Fact]
    public void ShouldNotHaveError_WhenSearchQueryLengthIsLessThanOrEqualTo100()
    {
        // Arrange
        var queryParameters = new TestQueryParameters { SearchQuery = new string('a', 100) };

        // Act
        var result = _validator.TestValidate(queryParameters);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SearchQuery);
    }

    /// <summary>
    ///     Tests that validation should have error when sort direction is not valid enum value.
    /// </summary>
    [Fact]
    public void ShouldHaveError_WhenSortDirectionIsNotValidEnumValue()
    {
        // Arrange
        var queryParameters = new TestQueryParameters { SortDirection = (SortDirection)(-1) };

        // Act
        var result = _validator.TestValidate(queryParameters);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SortDirection);
    }

    /// <summary>
    ///     Tests that validation should not have error when sort direction is valid enum value.
    /// </summary>
    [Fact]
    public void ShouldNotHaveError_WhenSortDirectionIsValidEnumValue()
    {
        // Arrange
        var queryParameters = new TestQueryParameters { SortDirection = SortDirection.Asc };

        // Act
        var result = _validator.TestValidate(queryParameters);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SortDirection);
    }

    /// <summary>
    ///     Tests that validation should have error when SortBy length is greater than 50.
    /// </summary>
    [Fact]
    public void ShouldHaveError_WhenSortByLengthIsGreaterThan50()
    {
        var queryParameters = new TestQueryParameters { SortBy = new string('a', 51) };
        var result = _validator.TestValidate(queryParameters);
        result.ShouldHaveValidationErrorFor(x => x.SortBy);
    }

    /// <summary>
    ///     Tests that validation should not have error when SortBy length is less than or equal to 50.
    /// </summary>
    [Fact]
    public void ShouldNotHaveError_WhenSortByLengthIsLessThanOrEqualTo50()
    {
        var queryParameters = new TestQueryParameters { SortBy = new string('a', 50) };
        var result = _validator.TestValidate(queryParameters);
        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }

    /// <summary>
    ///     TestQueryParameters record.
    /// </summary>
    private record TestQueryParameters : QueryParameters;

    /// <summary>
    ///     TestQueryValidator class.
    /// </summary>
    private class TestQueryValidator : QueryValidator<TestQueryParameters>
    {
    }
}