using Application.Identity.Commands.RegisterUser;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Identity.Commands.RegisterUser;

/// <summary>
///     Tests for the <see cref="RegisterUserCommandValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class RegisterUserCommandValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly RegisterUserCommandValidator _validator;

    /// <summary>
    ///     Setups RegisterUserCommandValidatorTests.
    /// </summary>
    public RegisterUserCommandValidatorTests()
    {
        _validator = new RegisterUserCommandValidator();
    }

    /// <summary>
    ///     Tests that validation should have error for Username when Username is empty.
    /// </summary>
    [Fact]
    public void RegisterUserCommand_ShouldHaveValidationErrorForUsername_WhenUsernameIsEmpty()
    {
        // Arrange
        var command = new RegisterUserCommand
        {
            Username = ""
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    /// <summary>
    ///     Tests that validation should have error for Username when Username length is greater than maximum.
    /// </summary>
    [Fact]
    public void RegisterUserCommand_ShouldHaveValidationErrorForUsername_WhenUsernameLengthIsGreaterThanMaximum()
    {
        // Arrange
        var command = new RegisterUserCommand
        {
            Username = new string('a', 21)
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    /// <summary>
    ///     Tests that validation should have error for username when username length is less than minimum.
    /// </summary>
    [Fact]
    public void RegisterUserCommand_ShouldHaveValidationErrorForUsername_WhenUsernameLengthIsLessThanMinimum()
    {
        // Arrange
        var command = new RegisterUserCommand { Username = new string('a', 2) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    /// <summary>
    ///     Tests that validation should not have error for Username when Username is valid.
    /// </summary>
    [Fact]
    public void RegisterUserCommand_ShouldNotHaveValidationErrorForUsername_WhenUsernameIsValid()
    {
        // Arrange
        var command = new RegisterUserCommand { Username = "username" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Username);
    }
}