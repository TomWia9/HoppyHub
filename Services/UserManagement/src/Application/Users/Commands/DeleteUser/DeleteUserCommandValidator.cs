using FluentValidation;
using SharedUtilities.Interfaces;

namespace Application.Users.Commands.DeleteUser;

/// <summary>
///     DeleteUserCommand validator.
/// </summary>
public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    /// <summary>
    ///     Initializes DeleteUserCommandValidator.
    /// </summary>
    /// <param name="currentUserService">Current user service</param>
    public DeleteUserCommandValidator(ICurrentUserService currentUserService)
    {
        RuleFor(x => x.Password)
            .NotEmpty().When(_ => !currentUserService.AdministratorAccess, ApplyConditionTo.CurrentValidator)
            .MaximumLength(256);
    }
}