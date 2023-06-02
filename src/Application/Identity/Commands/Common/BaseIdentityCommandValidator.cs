using FluentValidation;

namespace Application.Identity.Commands.Common;

/// <summary>
///     BaseIdentityCommand abstract validator.
/// </summary>
public abstract class BaseIdentityCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : BaseIdentityCommand
{
    /// <summary>
    ///     Initializes BaseIdentityCommandValidator.
    /// </summary>
    protected BaseIdentityCommandValidator()
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