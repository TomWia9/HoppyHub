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
    ///     Tests that validation should have error for empty email.
    /// </summary>
    [Fact]
    public void ShouldHaveValidationErrorForEmptyEmail()
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
    ///     Tests that validation should have error for invalid email.
    /// </summary>
    [Fact]
    public void ShouldHaveValidationErrorForInvalidEmailFormat()
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
    ///     Tests that validation should have error for email exceeding maximum length.
    /// </summary>
    [Fact]
    public void ShouldHaveValidationErrorForEmailExceedingMaxLength()
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
    ///     Tests that validation should not have error for email when email is valid.
    /// </summary>
    [Fact]
    public void ShouldNotHaveErrorWhenEmailIsValid()
    {
        // Arrange
        var command = new LoginUserCommand { Email = "test@example.com" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    /// <summary>
    ///     Tests that validation should have error for empty password.
    /// </summary>
    [Fact]
    public void ShouldHaveValidationErrorForEmptyPassword()
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
    ///     Tests that validation should have error for password exceeding maximum length.
    /// </summary>
    [Fact]
    public void ShouldHaveValidationErrorForPasswordExceedingMaxLength()
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
    ///     Tests that validation should not have error for password when password is valid.
    /// </summary>
    [Fact]
    public void ShouldNotHaveErrorWhenPasswordIsValid()
    {
        // Arrange
        var command = new LoginUserCommand() { Password = "password" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }
}