using Application.Identity.Commands.Common;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Identity.Commands.Common;

/// <summary>
///     Unit tests for the <see cref="BaseIdentityCommandValidator{TCommand}"/> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class BaseIdentityCommandValidatorTests
{
    /// <summary>
    ///     The TestBaseIdentity command.
    /// </summary>
    private record TestBaseIdentityCommand : BaseIdentityCommand;

    /// <summary>
    ///     The TestBaseIdentityCommand validator.
    /// </summary>
    private class TestBaseIdentityCommandValidator : BaseIdentityCommandValidator<TestBaseIdentityCommand>
    {
    }

    /// <summary>
    ///     The TestBaseIdentityCommand validator instance.
    /// </summary>
    private readonly TestBaseIdentityCommandValidator _validator;

    /// <summary>
    ///     Setups BaseIdentityCommandValidatorTests.
    /// </summary>
    public BaseIdentityCommandValidatorTests()
    {
        _validator = new TestBaseIdentityCommandValidator();
    }

    /// <summary>
    ///     Tests that validation should have error for Email when Email is empty.
    /// </summary>
    [Fact]
    public void BaseIdentityCommand_ShouldHaveValidationErrorForEmail_WhenEmailIsEmpty()
    {
        // Arrange
        var command = new TestBaseIdentityCommand
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
    public void BaseIdentityCommand_ShouldHaveValidationErrorForEmail_WhenEmailIsInvalid()
    {
        // Arrange
        var command = new TestBaseIdentityCommand
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
    public void BaseIdentityCommand_ShouldHaveValidationErrorForEmail_WhenEmailLengthIsGreaterThanMaximum()
    {
        // Arrange
        var email = string.Join("", Enumerable.Repeat("a", 257));
        var command = new TestBaseIdentityCommand
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
    public void BaseIdentityCommand_ShouldNotHaveErrorForEmail_WhenEmailIsValid()
    {
        // Arrange
        var command = new TestBaseIdentityCommand { Email = "test@example.com" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Email);
    }

    /// <summary>
    ///     Tests that validation should have error for Password when Password is empty.
    /// </summary>
    [Fact]
    public void BaseIdentityCommand_ShouldHaveValidationErrorForPassword_WhenPasswordIsEmpty()
    {
        // Arrange
        var command = new TestBaseIdentityCommand()
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
    public void BaseIdentityCommand_ShouldHaveValidationErrorForPassword_WhenPasswordLengthIsGreaterThanMaximum()
    {
        // Arrange
        var password = string.Join("", Enumerable.Repeat("a", 257));
        var command = new TestBaseIdentityCommand
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
    public void BaseIdentityCommand_ShouldNotHaveValidationErrorForPassword_WhenPasswordIsValid()
    {
        // Arrange
        var command = new TestBaseIdentityCommand() { Password = "password" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Password);
    }
}