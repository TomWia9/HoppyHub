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
        RuleFor(x => x.SortBy)
            .Must(value =>
                string.IsNullOrWhiteSpace(value) ||
                FavoritesFilteringHelper.SortingColumns.ContainsKey(value.ToUpper()))
            .WithMessage($"SortBy must be in [{string.Join(", ", FavoritesFilteringHelper.SortingColumns.Keys)}]");
    }
}