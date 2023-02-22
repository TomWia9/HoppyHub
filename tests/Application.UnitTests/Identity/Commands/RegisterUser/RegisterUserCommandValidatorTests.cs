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
    ///     Tests that validation should have error for empty email.
    /// </summary>
    [Fact]
    public void ShouldHaveValidationErrorForEmptyEmail()
    {
        // Arrange
        var command = new RegisterUserCommand
        {
            Email = "",
            Username = "username",
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
        var command = new RegisterUserCommand
        {
            Email = "invalid_email_format",
            Username = "username",
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
        var command = new RegisterUserCommand
        {
            Email = email,
            Username = "username",
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
        var command = new RegisterUserCommand { Email = "test@example.com" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    /// <summary>
    ///     Tests that validation should have error for empty username.
    /// </summary>
    [Fact]
    public void ShouldHaveValidationErrorForEmptyUsername()
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
    ///     Tests that validation should have error for username exceeding maximum length.
    /// </summary>
    [Fact]
    public void ShouldHaveValidationErrorForUsernameExceedingMaxLength()
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
    ///     Tests that validation should not have error for username when username is valid.
    /// </summary>
    [Fact]
    public void ShouldNotHaveErrorWhenUsernameIsValid()
    {
        // Arrange
        var command = new RegisterUserCommand { Username = "username" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Username);
    }

    /// <summary>
    ///     Tests that validation should have error for empty password.
    /// </summary>
    [Fact]
    public void ShouldHaveValidationErrorForEmptyPassword()
    {
        // Arrange
        var command = new RegisterUserCommand
        {
            Email = "email",
            Username = "username",
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
        var command = new RegisterUserCommand
        {
            Email = "email",
            Username = "username",
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
        var command = new RegisterUserCommand { Password = "password" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }
}