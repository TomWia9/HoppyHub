using FluentValidation;
using SharedUtilities.Interfaces;

namespace Application.Users.Commands.UpdateUserPassword;

/// <summary>
///     UpdateUserPasswordCommand validator.
/// </summary>
public class UpdateUserPasswordCommandValidator: AbstractValidator<UpdateUserPasswordCommand>
{
    /// <summary>
    ///     Initializes UpdateUserPasswordCommandValidator.
    /// </summary>
    /// <param name="currentUserService">Current user service</param>
    public UpdateUserPasswordCommandValidator(ICurrentUserService currentUserService)
    {
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