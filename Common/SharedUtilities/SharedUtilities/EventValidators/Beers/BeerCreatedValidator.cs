using FluentValidation;
using SharedEvents.Events;

namespace SharedUtilities.EventValidators.Beers;

/// <summary>
///     BeerCreated event validator.
/// </summary>
public class BeerCreatedValidator : AbstractValidator<BeerCreated>
{
    /// <summary>
    ///     Initializes BeerCreatedValidator.
    /// </summary>
    public BeerCreatedValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.BreweryName)
            .NotEmpty()
            .MaximumLength(500);
    }
}