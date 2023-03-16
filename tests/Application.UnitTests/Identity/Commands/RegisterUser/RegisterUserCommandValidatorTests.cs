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
    ///     Tests that validation should have error for Email when Email is Empty.
    /// </summary>
    [Fact]
    public void RegisterUserCommand_ShouldHaveValidationErrorForEmail_WhenEmailIsEmpty()
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
    ///     Tests that validation should have error for Email when Email is invalid.
    /// </summary>
    [Fact]
    public void RegisterUserCommand_ShouldHaveValidationErrorForEmail_WhenEmailIsInvalid()
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
    ///     Tests that validation should have error for Email when Email length is greater than maximum.
    /// </summary>
    [Fact]
    public void RegisterUserCommand_ShouldHaveValidationErrorForEmail_WhenEmailLengthIsGreaterThanMaximum()
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
    ///     Tests that validation should not have error for Email when Email is valid.
    /// </summary>
    [Fact]
    public void RegisterUserCommand_ShouldNotHaveValidationErrorForEmail_WhenEmailIsValid()
    {
        // Arrange
        var command = new RegisterUserCommand { Email = "test@example.com" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
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

    /// <summary>
    ///     Tests that validation should have error for Password when Password is empty.
    /// </summary>
    [Fact]
    public void RegisterUserCommand_ShouldHaveValidationErrorForPassword_WhenPasswordIsEmpty()
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
    ///     Tests that validation should have error for Password when Password length is greater than maximum.
    /// </summary>
    [Fact]
    public void RegisterUserCommand_ShouldHaveValidationErrorForPassword_WhenPasswordLengthIsGreaterThanMaximum()
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
    ///     Tests that validation should not have error for Password when Password is valid.
    /// </summary>
    [Fact]
    public void RegisterUserCommand_ShouldNotHaveValidationErrorForPassword_WhenPasswordIsValid()
    {
        // Arrange
        var command = new RegisterUserCommand { Password = "password" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }
}