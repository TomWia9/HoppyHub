using FluentValidation;

namespace Application.Beers.Commands.Common;

/// <summary>
///     BaseBeerCommand abstract validator.
/// </summary>
public abstract class BaseBeerCommandValidator<TCommand> : AbstractValidator<TCommand> where TCommand : BaseBeerCommand
{
    protected const string UniqueNameErrorMessage = "The beer name must be unique within the brewery.";

    /// <summary>
    ///     Initializes BaseBeerCommandValidator.
    /// </summary>
    protected BaseBeerCommandValidator()
    {
        RuleFor(x => x.BreweryId).NotEmpty();
        RuleFor(x => x.AlcoholByVolume).NotNull().InclusiveBetween(0, 100);
        RuleFor(x => x.Description).MaximumLength(3000);
        RuleFor(x => x.Composition).MaximumLength(300);
        RuleFor(x => x.Blg).InclusiveBetween(0, 100);
        RuleFor(x => x.BeerStyleId).NotEmpty();
        RuleFor(x => x.Ibu).InclusiveBetween(0, 200);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}