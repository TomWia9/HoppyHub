using FluentValidation;
using SharedEvents.Events;

namespace SharedUtilities.EventValidators.Beers;

/// <summary>
///     BeerUpdated event validator.
/// </summary>
public class BeerUpdatedValidator : AbstractValidator<BeerUpdated>
{
    /// <summary>
    ///     Initializes BeerUpdatedValidator.
    /// </summary>
    public BeerUpdatedValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.BreweryName)
            .NotEmpty()
            .MaximumLength(500);
    }
}