using FluentValidation;
using SharedEvents.Events;

namespace SharedUtilities.EventValidators.Beers;

/// <summary>
///     BeerFavoritesCountChanged event validator.
/// </summary>
public class BeerFavoritesCountChangedValidator : AbstractValidator<BeerFavoritesCountChanged>
{
    /// <summary>
    ///     Initializes BeerFavoritesCountChangedValidator.
    /// </summary>
    public BeerFavoritesCountChangedValidator()
    {
        RuleFor(x => x.FavoritesCount).GreaterThanOrEqualTo(0);
    }
}