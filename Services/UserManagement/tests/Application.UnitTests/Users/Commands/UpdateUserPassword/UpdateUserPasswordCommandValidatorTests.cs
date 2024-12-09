using Application.Users.Commands.UpdateUserPassword;
using FluentValidation.TestHelper;
using Moq;
using SharedUtilities.Interfaces;

namespace Application.UnitTests.Users.Commands.UpdateUserPassword;

/// <summary>
///     Unit tests for the <see cref="UpdateUserPasswordCommandValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class UpdateUserPasswordPasswordCommandValidatorTests
{
    /// <summary>
    ///     The current user service mock.
    /// </summary>
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly UpdateUserPasswordCommandValidator _validator;

    /// <summary>
    ///     Setups UpdateUserPasswordCommandValidatorTests.
    /// </summary>
    public UpdateUserPasswordPasswordCommandValidatorTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _validator = new UpdateUserPasswordCommandValidator(_currentUserServiceMock.Object);
    }
    
    /// <summary>
    ///     Tests that validation should have error for CurrentPassword when CurrentPassword length is greater than maximum.
    /// </summary>
    [Fact]
    public void
        UpdateUserPasswordCommand_ShouldHaveValidationErrorForCurrentPassword_WhenCurrentPasswordLengthIsGreaterThanMaximum()
    {
        // Arrange
        var command = new UpdateUserPasswordCommand { CurrentPassword = new string('a', 257) };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.CurrentPassword);
    }

    /// <summary>
    ///     Tests that validation should not have error for CurrentPassword when CurrentPassword is valid.
    /// </summary>
    [Fact]
    public void UpdateUserPasswordCommand_ShouldNotHaveValidationErrorForCurrentPassword_WhenCurrentPasswordIsValid()
    {
        // Arrange
        var command = new UpdateUserPasswordCommand { CurrentPassword = "password" };

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
        UpdateUserPasswordCommand_ShouldNotHaveValidationErrorForNewPassword_WhenNewPasswordPresentAndCurrentPasswordPresentAndAdministratorAccessTrue()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(true);
        var command = new UpdateUserPasswordCommand { CurrentPassword = "currentPassword", NewPassword = "newPassword" };

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
        UpdateUserPasswordCommand_ShouldHaveValidationErrorForCurrentPassword_CurrentPasswordNotPresentAndNewPasswordPresentAndAndAdministratorAccessFalse()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);
        var command = new UpdateUserPasswordCommand { NewPassword = "newPassword" };

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
        UpdateUserPasswordCommand_ShouldNotHaveValidationErrorForNewPassword_WhenCurrentPasswordPresentAndNewPasswordPresentAndAdministratorAccessFalse()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);
        var command = new UpdateUserPasswordCommand { CurrentPassword = "currentPassword", NewPassword = "newPassword" };

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
        UpdateUserPasswordCommand_ShouldHaveValidationErrorForNewPassword_WhenNewPasswordNotPresentAndCurrentPasswordPresentAndAdministratorAccessFalse()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);
        var command = new UpdateUserPasswordCommand { CurrentPassword = "currentPassword" };

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
        UpdateUserPasswordCommand_ShouldNotHaveValidationErrorForNewPassword_WhenNewPasswordDifferentFromCurrentPassword()
    {
        // Arrange
        var command = new UpdateUserPasswordCommand { CurrentPassword = "currentPassword", NewPassword = "newPassword" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.NewPassword);
    }

    /// <summary>
    ///     Tests that validation should have error when new password is same as current password.
    /// </summary>
    [Fact]
    public void UpdateUserPasswordCommand_ShouldHaveValidationErrorForNewPassword_WhenNewPasswordSameAsCurrentPassword()
    {
        // Arrange
        var command = new UpdateUserPasswordCommand { CurrentPassword = "password", NewPassword = "password" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.NewPassword);
    }
}