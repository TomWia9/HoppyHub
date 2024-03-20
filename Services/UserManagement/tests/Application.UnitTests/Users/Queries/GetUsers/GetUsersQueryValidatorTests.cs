using Application.Users.Dtos;
using Application.Users.Queries.GetUsers;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Users.Queries.GetUsers;

/// <summary>
///     Tests for the <see cref="GetUsersQueryValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class GetUsersQueryValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly GetUsersQueryValidator _validator;

    /// <summary>
    ///     Setups GetUsersQueryValidatorTests.
    /// </summary>
    public GetUsersQueryValidatorTests()
    {
        _validator = new GetUsersQueryValidator();
    }

    /// <summary>
    ///     Tests that validation should have error for role when role exceeds maximum length.
    /// </summary>
    [Fact]
    public void GetUsersQuery_ShouldHaveValidationErrorForRole_WhenRoleExceedsMaximumLength()
    {
        // Arrange
        var query = new GetUsersQuery
        {
            Role = new string('x', 16)
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Role);
    }

    /// <summary>
    ///     Tests that validation should not have error for role when role is valid.
    /// </summary>
    [Fact]
    public void GetUsersQuery_ShouldNotHaveValidationErrorForRole_WhenRoleIsValid()
    {
        // Arrange
        var query = new GetUsersQuery
        {
            Role = "User"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Role);
    }

    /// <summary>
    ///     Tests that validation should have error for SortBy when SortBy is not allowed column.
    /// </summary>
    [Fact]
    public void GetUsersQuery_ShouldHaveValidationErrorForSortBy_WhenSortByIsNotAllowedColumn()
    {
        // Arrange
        var query = new GetUsersQuery
        {
            SortBy = "invalid_column"
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.SortBy)
            .WithErrorMessage("SortBy must be in [EMAIL, USERNAME]");
    }

    /// <summary>
    ///     Tests that validation should not have error for SortBy when SortBy is valid.
    /// </summary>
    /// <param name="sortBy">The column by which to sort</param>
    [Theory]
    [InlineData(nameof(UserDto.Email))]
    [InlineData(nameof(UserDto.Username))]
    [InlineData("")]
    [InlineData(null)]
    public void GetUsersQuery_ShouldNotHaveValidationErrorForSortBy_WhenSortByIsAllowedColumn(string? sortBy)
    {
        // Arrange
        var query = new GetUsersQuery
        {
            SortBy = sortBy
        };

        // Act
        var result = _validator.TestValidate(query);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.SortBy);
    }
}