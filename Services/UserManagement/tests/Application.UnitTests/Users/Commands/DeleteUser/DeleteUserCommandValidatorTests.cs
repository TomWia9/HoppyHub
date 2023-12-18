using Application.Users.Commands.DeleteUser;
using FluentValidation.TestHelper;
using Moq;
using SharedUtilities.Interfaces;

namespace Application.UnitTests.Users.Commands.DeleteUser;

/// <summary>
///     Unit tests for the <see cref="DeleteUserCommandValidator" /> class.
/// </summary>
[ExcludeFromCodeCoverage]
public class DeleteUserCommandValidatorTests
{
    /// <summary>
    ///     The current user service mock.
    /// </summary>
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;

    /// <summary>
    ///     The validator.
    /// </summary>
    private readonly DeleteUserCommandValidator _validator;

    /// <summary>
    ///     Setups DeleteUserCommandValidatorTests.
    /// </summary>
    public DeleteUserCommandValidatorTests()
    {
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        _validator = new DeleteUserCommandValidator(_currentUserServiceMock.Object);
    }

    /// <summary>
    ///     Tests that validation should not have error for Password when Password is valid.
    /// </summary>
    [Fact]
    public void DeleteUserCommand_ShouldNotHaveErrorForPassword_WhenPasswordIsValid()
    {
        // Arrange
        var command = new DeleteUserCommand
        {
            Password = "ValidPassword"
        };

        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    /// <summary>
    ///     Tests that validation should have error for Password when Password is empty and user is not administrator.
    /// </summary>
    [Fact]
    public void DeleteUserCommand_ShouldHaveErrorForPassword_WhenPasswordIsEmptyAndUserIsNotAdministrator()
    {
        // Arrange
        var command = new DeleteUserCommand
        {
            Password = string.Empty
        };

        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    /// <summary>
    ///     Tests that validation should have error for password when password exceeds maximum length.
    /// </summary>
    [Fact]
    public void DeleteUserCommand_ShouldHaveErrorForPassword_WhenPasswordExceedsMaximumLength()
    {
        // Arrange
        var command = new DeleteUserCommand
        {
            Password = new string('x', 257)
        };

        _currentUserServiceMock.Setup(x => x.AdministratorAccess).Returns(false);

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}