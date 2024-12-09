using Application.Users.Commands.UpdateUsername;
using FluentValidation.TestHelper;

namespace Application.UnitTests.Users.Commands.UpdateUser;

/// <summary>
///     Unit tests for the <see cref="UpdateUsernameCommandValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateUsernameCommandValidatorTests
{
    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly UpdateUsernameCommandValidator _validator;

    /// <summary>
    ///     Setups UpdateUsernameCommandValidatorTests.
    /// </summary>
    public UpdateUsernameCommandValidatorTests()
    {
        _validator = new UpdateUsernameCommandValidator();
    }

    /// <summary>
    ///     Tests that validation should not have error for Username when Username length is valid.
    /// </summary>
    [Fact]
    public void UpdateUsernameCommand_ShouldNotHaveValidationErrorForUsername_WhenUsernameLengthIsValid()
    {
        // Arrange
        var command = new UpdateUsernameCommand { Username = "test" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Username);
    }

    /// <summary>
    ///     Tests that validation should have error for username when username length is greater than maximum.
    /// </summary>
    [Fact]
    public void UpdateUserCommand_ShouldHaveValidationErrorForUsername_WhenUsernameLengthIsGreaterThanMaximum()
    {
        // Arrange
        var command = new UpdateUsernameCommand { Username = new string('a', 257) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }
}