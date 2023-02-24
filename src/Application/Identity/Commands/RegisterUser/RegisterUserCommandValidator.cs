using FluentValidation;

namespace Application.Identity.Commands.RegisterUser;

/// <summary>
///     RegisterUserCommand validator.
/// </summary>
public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    /// <summary>
    ///     Initializes RegisterUserCommandValidator.
    /// </summary>
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256);

        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(256);
    }
}