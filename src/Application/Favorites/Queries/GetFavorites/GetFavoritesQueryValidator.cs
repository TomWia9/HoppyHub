using Application.Common.Abstractions;
using FluentValidation;

namespace Application.Favorites.Queries.GetFavorites;

/// <summary>
///     GetFavoritesQuery validator.
/// </summary>
public class GetFavoritesQueryValidator : QueryValidator<GetFavoritesQuery>
{
    /// <summary>
    ///     Initializes GetFavoritesQueryValidator.
    /// </summary>
    public GetFavoritesQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}