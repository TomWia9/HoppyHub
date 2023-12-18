using FluentValidation.TestHelper;
using SharedEvents.Events;
using SharedUtilities.EventValidators.Users;
using SharedUtilities.Models;

namespace SharedUtilities.UnitTests.EventValidators.Users;

/// <summary>
///     Unit tests for the <see cref="UserCreatedValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UserCreatedValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly UserCreatedValidator _validator;

    /// <summary>
    ///     Setups UserCreatedValidatorTests.
    /// </summary>
    public UserCreatedValidatorTests()
    {
        _validator = new UserCreatedValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for Username when Username is valid.
    /// </summary>
    [Fact]
    public void UserCreated_ShouldNotHaveValidationErrorForUsername_WhenUsernameIsValid()
    {
        // Arrange
        var userCreatedEvent = new UserCreated
        {
            Username = new string('x', 15)
        };

        // Act
        var result = _validator.TestValidate(userCreatedEvent);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Username);
    }

    /// <summary>
    ///     Tests that validation should have error for Username when Username exceeds maximum length.
    /// </summary>
    [Fact]
    public void UserCreated_ShouldHaveValidationErrorForUsername_WhenUsernameExceedsMaximumLength()
    {
        // Arrange
        var userCreatedEvent = new UserCreated
        {
            Username = new string('x', 257)
        };

        // Act
        var result = _validator.TestValidate(userCreatedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    /// <summary>
    ///     Tests that validation should have error for Username when Username is empty.
    /// </summary>
    [Fact]
    public void UserCreated_ShouldHaveValidationErrorForUsername_WhenUsernameIsEmpty()
    {
        // Arrange
        var userCreatedEvent = new UserCreated
        {
            Username = string.Empty
        };

        // Act
        var result = _validator.TestValidate(userCreatedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    /// <summary>
    ///     Tests that validation should not have error for Role when Role is valid.
    /// </summary>
    [Fact]
    public void UserCreated_ShouldNotHaveValidationErrorForRole_WhenRoleIsValid()
    {
        // Arrange
        var userCreatedEvent = new UserCreated
        {
            Id = Guid.NewGuid(),
            Role = Roles.User
        };

        // Act
        var result = _validator.TestValidate(userCreatedEvent);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Role);
    }

    /// <summary>
    ///     Tests that validation should have error for Role when Role is invalid.
    /// </summary>
    [Fact]
    public void UserCreated_ShouldHaveValidationErrorForRole_WhenRoleIsInvalid()
    {
        // Arrange
        var userCreatedEvent = new UserCreated
        {
            Id = Guid.NewGuid(),
            Role = "testRole"
        };

        // Act
        var result = _validator.TestValidate(userCreatedEvent);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Role);
    }
}