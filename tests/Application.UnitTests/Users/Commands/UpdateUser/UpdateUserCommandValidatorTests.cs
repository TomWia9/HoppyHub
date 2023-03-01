using Application.Common.Interfaces;
using Application.Users.Commands.UpdateUser;
using Moq;

namespace Application.UnitTests.Users.Commands.UpdateUser;

/// <summary>
///     Unit tests for the <see cref="UpdateUserCommandValidator"/> class.
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
    ///     Tests that validation should not have error for username when username length is less than maximum.
    /// </summary>
    [Fact]
    public void ShouldNotHaveValidationError_WhenUsernameLengthIsLessThanMaximum()
    {
        // Arrange
        var command = new UpdateUserCommand { Username = "test" };

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    /// <summary>
    ///     Tests that validation should have error for username when username length is greater than maximum.
    /// </summary>
    [Fact]
    public void ShouldHaveValidationError_WhenUsernameLengthIsGreaterThanMaximum()
    {
        // Arrange
        var command = new UpdateUserCommand { Username = new string('a', 257) };

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(x => x.PropertyName == "Username" && x.ErrorMessage.Contains("256"));
    }

    /// <summary>
    ///     Tests that validation should not have error when new password present
    ///     and current password present and administrator access true.
    /// </summary>
    [Fact]
    public void ShouldNotHaveValidationError_WhenNewPasswordPresentAndCurrentPasswordPresentAndAdministratorAccessTrue()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(true);
        var command = new UpdateUserCommand { CurrentPassword = "currentPassword", NewPassword = "newPassword" };

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    /// <summary>
    ///     Tests that validation should have error when new password present
    ///     and current password not present and administrator access false.
    /// </summary>
    [Fact]
    public void
        ShouldHaveValidationError_WhenNewPasswordPresentAndCurrentPasswordNotPresentAndAdministratorAccessFalse()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);
        var command = new UpdateUserCommand { NewPassword = "newPassword" };

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    /// <summary>
    ///     Tests that validation should not have error when new password present
    ///     and current password present and administrator access false.
    /// </summary>
    [Fact]
    public void
        ShouldNotHaveValidationError_WhenCurrentPasswordPresentAndNewPasswordPresentAndAdministratorAccessFalse()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);
        var command = new UpdateUserCommand { CurrentPassword = "currentPassword", NewPassword = "newPassword" };

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    /// <summary>
    ///     Tests that validation should have error when new password not present
    ///     and current password present and administrator access false.
    /// </summary>
    [Fact]
    public void
        ShouldHaveValidationError_WhenNewPasswordNotPresentAndCurrentPasswordPresentAndAdministratorAccessFalse()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);
        var command = new UpdateUserCommand { CurrentPassword = "currentPassword" };

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
    }

    /// <summary>
    ///     Tests that validation should not have error when new password is different from current password.
    /// </summary>
    [Fact]
    public void ShouldNotHaveValidationError_WhenNewPasswordDifferentFromCurrentPassword()
    {
        // Arrange
        var command = new UpdateUserCommand { CurrentPassword = "currentPassword", NewPassword = "newPassword" };
        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeTrue();
    }

    /// <summary>
    ///     Tests that validation should have error when new password is same as current password.
    /// </summary>
    [Fact]
    public void ShouldHaveValidationError_WhenNewPasswordSameAsCurrentPassword()
    {
        // Arrange
        var command = new UpdateUserCommand { CurrentPassword = "password", NewPassword = "password" };

        // Act
        var validationResult = _validator.Validate(command);

        // Assert
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should()
            .Contain(x => x.PropertyName == "NewPassword" && x.ErrorMessage.Contains("different"));
    }
}