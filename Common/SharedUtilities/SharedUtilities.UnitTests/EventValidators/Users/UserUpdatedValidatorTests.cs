using FluentValidation.TestHelper;
using SharedEvents.Events;
using SharedUtilities.EventValidators.Users;

namespace SharedUtilities.UnitTests.EventValidators.Users;

/// <summary>
///     Unit tests for the <see cref="UserUpdatedValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UserUpdatedValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly UserUpdatedValidator _validator;

    /// <summary>
    ///     Setups UserUpdatedValidatorTests.
    /// </summary>
    public UserUpdatedValidatorTests()
    {
        _validator = new UserUpdatedValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for Username when Username is valid.
    /// </summary>
    [Fact]
    public void UserUpdated_ShouldNotHaveValidationErrorForUsername_WhenUsernameIsValid()
    {
        // Arrange
        var userUpdatedEvent = new UserUpdated
        {
            Username = new string('x', 15)
        };

        // Act
        var result = _validator.TestValidate(userUpdatedEvent);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Username);
    }

    /// <summary>
    ///     Tests that validation should have error for Username when Username exceeds maximum length.
    /// </summary>
    [Fact]
    public void UserUpdated_ShouldHaveValidationErrorForUsername_WhenUsernameExceedsMaximumLength()
    {
        // Arrange
        var userUpdatedEvent = new UserUpdated
        {
            Username = new string('x', 257)
        };

        // Act
        var result = _validator.TestValidate(userUpdatedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    /// <summary>
    ///     Tests that validation should have error for Username when Username is empty.
    /// </summary>
    [Fact]
    public void UserUpdated_ShouldHaveValidationErrorForUsername_WhenUsernameIsEmpty()
    {
        // Arrange
        var userUpdatedEvent = new UserUpdated
        {
            Username = string.Empty
        };

        // Act
        var result = _validator.TestValidate(userUpdatedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }
}