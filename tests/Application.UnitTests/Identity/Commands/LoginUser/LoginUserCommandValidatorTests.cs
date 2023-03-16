using Application.Identity.Commands.LoginUser;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Identity.Commands.LoginUser;

/// <summary>
///     Tests for the <see cref="LoginUserCommandValidator"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class LoginUserCommandValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly LoginUserCommandValidator _validator;

    /// <summary>
    ///     Setups LoginUserCommandValidatorTests.
    /// </summary>
    public LoginUserCommandValidatorTests()
    {
        _validator = new LoginUserCommandValidator();
    }

    /// <summary>
    ///     Tests that validation should have error for Email when email is empty.
    /// </summary>
    [Fact]
    public void LoginUserCommand_ShouldHaveValidationErrorForEmail_WhenEmailIsEmpty()
    {
        // Arrange
        var command = new LoginUserCommand
        {
            Email = "",
            Password = "password"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    /// <summary>
    ///     Tests that validation should have error for Email when Email is invalid.
    /// </summary>
    [Fact]
    public void LoginUserCommand_ShouldHaveValidationErrorForEmail_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new LoginUserCommand
        {
            Email = "invalid_email_format",
            Password = "password"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    /// <summary>
    ///     Tests that validation should have error for Email when Email length is greater than maximum.
    /// </summary>
    [Fact]
    public void LoginUserCommand_ShouldHaveValidationErrorForEmail_WhenEmailLengthIsGreaterThanMaximum()
    {
        // Arrange
        var email = string.Join("", Enumerable.Repeat("a", 257));
        var command = new LoginUserCommand
        {
            Email = email,
            Password = "password"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    /// <summary>
    ///     Tests that validation should not have error for Email when Email is valid.
    /// </summary>
    [Fact]
    public void LoginUserCommand_ShouldNotHaveErrorForEmail_WhenEmailIsValid()
    {
        // Arrange
        var command = new LoginUserCommand { Email = "test@example.com" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    /// <summary>
    ///     Tests that validation should have error for Password when Password is empty.
    /// </summary>
    [Fact]
    public void LoginUserCommand_ShouldHaveValidationErrorForPassword_WhenPasswordIsEmpty()
    {
        // Arrange
        var command = new LoginUserCommand()
        {
            Email = "email",
            Password = ""
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    /// <summary>
    ///     Tests that validation should have error for Password when Password length is greater than maximum.
    /// </summary>
    [Fact]
    public void LoginUserCommand_ShouldHaveValidationErrorForPassword_WhenPasswordLengthIsGreaterThanMaximum()
    {
        // Arrange
        var password = string.Join("", Enumerable.Repeat("a", 257));
        var command = new LoginUserCommand
        {
            Email = "email",
            Password = password
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    /// <summary>
    ///     Tests that validation should not have error for Password when Password is valid.
    /// </summary>
    [Fact]
    public void LoginUserCommand_ShouldNotHaveValidationErrorForPassword_WhenPasswordIsValid()
    {
        // Arrange
        var command = new LoginUserCommand() { Password = "password" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }
}