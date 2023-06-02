using FluentValidation;

namespace Application.BeerStyles.Commands.Common;

/// <summary>
///     BaseBeerStyleCommand abstract validator.
/// </summary>
public abstract class BaseBeerStyleCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : BaseBeerStyleCommand
{
    protected const string UniqueNameErrorMessage = "The beer style name must be unique.";

    /// <summary>
    ///     Initializes BaseBeerStyleCommandValidator.
    /// </summary>
    protected BaseBeerStyleCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.CountryOfOrigin).NotEmpty().MaximumLength(50);
    }
}