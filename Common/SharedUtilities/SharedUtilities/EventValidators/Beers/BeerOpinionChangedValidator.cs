using FluentValidation;
using SharedEvents.Events;

namespace SharedUtilities.EventValidators.Beers;

/// <summary>
///     BeerOpinionChanged event validator.
/// </summary>
public class BeerOpinionChangedValidator : AbstractValidator<BeerOpinionChanged>
{
    /// <summary>
    ///     Initializes BeerOpinionChangedValidator.
    /// </summary>
    public BeerOpinionChangedValidator()
    {
        RuleFor(x => x.OpinionsCount).GreaterThanOrEqualTo(0);
        RuleFor(x => x.NewBeerRating).GreaterThanOrEqualTo(0);
    }
}