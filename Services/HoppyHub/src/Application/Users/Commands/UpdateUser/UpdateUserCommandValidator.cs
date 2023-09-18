using Application.Common.Interfaces;
using FluentValidation;

namespace Application.Users.Commands.UpdateUser;

/// <summary>
///     UpdateUserCommand validator.
/// </summary>
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    /// <summary>
    ///     Initializes UpdateUserCommandValidator.
    /// </summary>
    /// <param name="currentUserService">Current user service</param>
    public UpdateUserCommandValidator(ICurrentUserService currentUserService)
    {
        RuleFor(x => x.Username)
            .MaximumLength(256);

        RuleFor(x => x.CurrentPassword)
            .NotEmpty().When(x => !string.IsNullOrEmpty(x.NewPassword) && !currentUserService.AdministratorAccess,
                ApplyConditionTo.CurrentValidator)
            .MaximumLength(256);

        RuleFor(x => x.NewPassword)
            .NotEmpty().When(x => !string.IsNullOrEmpty(x.CurrentPassword) && !currentUserService.AdministratorAccess,
                ApplyConditionTo.CurrentValidator)
            .NotEqual(x => x.CurrentPassword).WithMessage("The new password must be different from the previous one.")
            .When(x => !string.IsNullOrEmpty(x.CurrentPassword), ApplyConditionTo.CurrentValidator)
            .MaximumLength(256);
    }
}