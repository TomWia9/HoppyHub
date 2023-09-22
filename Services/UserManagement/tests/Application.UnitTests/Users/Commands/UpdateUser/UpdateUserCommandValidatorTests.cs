using Application.Users.Commands.UpdateUser;
using FluentValidation.TestHelper;
using Moq;
using SharedUtilities.Interfaces;

namespace Application.UnitTests.Users.Commands.UpdateUser;

/// <summary>
///     Unit tests for the <see cref="UpdateUserCommandValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateUserCommandValidatorTests
{
    /// <summary>
    ///     The current user service mock.
    /// </summary>
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly UpdateUserCommandValidator _validator;

    /// <summary>
    ///     Setups UpdateUserCommandValidatorTests.
    /// </summary>
    public UpdateUserCommandValidatorTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _validator = new UpdateUserCommandValidator(_currentUserServiceMock.Object);
    }

    /// <summary>
    ///     Tests that validation should not have error for Username when Username length is valid.
    /// </summary>
    [Fact]
    public void UpdateUserCommand_ShouldNotHaveValidationErrorForUsername_WhenUsernameLengthIsValid()
    {
        // Arrange
        var command = new UpdateUserCommand { Username = "test" };

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
        var command = new UpdateUserCommand { Username = new string('a', 257) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Username);
    }

    /// <summary>
    ///     Tests that validation should have error for CurrentPassword when CurrentPassword length is greater than maximum.
    /// </summary>
    [Fact]
    public void
        UpdateUserCommand_ShouldHaveValidationErrorForCurrentPassword_WhenCurrentPasswordLengthIsGreaterThanMaximum()
    {
        // Arrange
        var command = new UpdateUserCommand { CurrentPassword = new string('a', 257) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CurrentPassword);
    }

    /// <summary>
    ///     Tests that validation should not have error for CurrentPassword when CurrentPassword is valid.
    /// </summary>
    [Fact]
    public void UpdateUserCommand_ShouldNotHaveValidationErrorForCurrentPassword_WhenCurrentPasswordIsValid()
    {
        // Arrange
        var command = new UpdateUserCommand { CurrentPassword = "password" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.CurrentPassword);
    }

    /// <summary>
    ///     Tests that validation should not have error when new password present
    ///     and current password present and administrator access true.
    /// </summary>
    [Fact]
    public void
        UpdateUserCommand_ShouldNotHaveValidationErrorForNewPassword_WhenNewPasswordPresentAndCurrentPasswordPresentAndAdministratorAccessTrue()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(true);
        var command = new UpdateUserCommand { CurrentPassword = "currentPassword", NewPassword = "newPassword" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.NewPassword);
    }

    /// <summary>
    ///     Tests that validation should have error when new password present
    ///     and current password not present and administrator access false.
    /// </summary>
    [Fact]
    public void
        UpdateUserCommand_ShouldHaveValidationErrorForCurrentPassword_CurrentPasswordNotPresentAndNewPasswordPresentAndAndAdministratorAccessFalse()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);
        var command = new UpdateUserCommand { NewPassword = "newPassword" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CurrentPassword);
    }

    /// <summary>
    ///     Tests that validation should not have error when new password present
    ///     and current password present and administrator access false.
    /// </summary>
    [Fact]
    public void
        UpdateUserCommand_ShouldNotHaveValidationErrorForNewPassword_WhenCurrentPasswordPresentAndNewPasswordPresentAndAdministratorAccessFalse()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);
        var command = new UpdateUserCommand { CurrentPassword = "currentPassword", NewPassword = "newPassword" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.NewPassword);
    }

    /// <summary>
    ///     Tests that validation should have error when new password not present
    ///     and current password present and administrator access false.
    /// </summary>
    [Fact]
    public void
        UpdateUserCommand_ShouldHaveValidationErrorForNewPassword_WhenNewPasswordNotPresentAndCurrentPasswordPresentAndAdministratorAccessFalse()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);
        var command = new UpdateUserCommand { CurrentPassword = "currentPassword" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }

    /// <summary>
    ///     Tests that validation should not have error when new password is different from current password.
    /// </summary>
    [Fact]
    public void
        UpdateUserCommand_ShouldNotHaveValidationErrorForNewPassword_WhenNewPasswordDifferentFromCurrentPassword()
    {
        // Arrange
        var command = new UpdateUserCommand { CurrentPassword = "currentPassword", NewPassword = "newPassword" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.NewPassword);
    }

    /// <summary>
    ///     Tests that validation should have error when new password is same as current password.
    /// </summary>
    [Fact]
    public void UpdateUserCommand_ShouldHaveValidationErrorForNewPassword_WhenNewPasswordSameAsCurrentPassword()
    {
        // Arrange
        var command = new UpdateUserCommand { CurrentPassword = "password", NewPassword = "password" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }
}