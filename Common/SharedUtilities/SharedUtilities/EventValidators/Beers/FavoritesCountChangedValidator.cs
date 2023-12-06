using FluentValidation;
using SharedEvents.Events;

namespace SharedUtilities.EventValidators.Beers;

/// <summary>
///     FavoritesCountChanged event validator.
/// </summary>
public class FavoritesCountChangedValidator : AbstractValidator<FavoritesCountChanged>
{
    /// <summary>
    ///     Initializes BeerOpinionChangedValidator.
    /// </summary>
    public FavoritesCountChangedValidator()
    {
        RuleFor(x => x.FavoritesCount).GreaterThanOrEqualTo(0);
    }
}