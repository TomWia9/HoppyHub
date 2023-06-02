using Application.Identity.Commands.RegisterUser;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Identity.Commands.RegisterUser;

/// <summary>
///     Tests for the <see cref="RegisterUserCommandValidator"/> class.
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
            Email = "email",
            Username = "",
            Password = "password"
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
        var username = string.Join("", Enumerable.Repeat("a", 257));
        var command = new RegisterUserCommand
        {
            Email = "email",
            Username = username,
            Password = "password"
        };

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