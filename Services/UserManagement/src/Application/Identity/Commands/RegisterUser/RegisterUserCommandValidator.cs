using Application.Identity.Commands.Common;
using FluentValidation;

namespace Application.Identity.Commands.RegisterUser;

/// <summary>
///     RegisterUserCommand validator.
/// </summary>
public class RegisterUserCommandValidator : BaseIdentityCommandValidator<RegisterUserCommand>
{
    /// <summary>
    ///     Initializes RegisterUserCommandValidator.
    /// </summary>
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(256);
    }
}