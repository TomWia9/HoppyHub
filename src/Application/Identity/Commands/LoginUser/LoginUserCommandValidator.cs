using FluentValidation;

namespace Application.Identity.Commands.LoginUser;

/// <summary>
///     LoginUserCommand validator.
/// </summary>
public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    /// <summary>
    ///     Initializes LoginUserCommandValidator.
    /// </summary>
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(256);
    }
}