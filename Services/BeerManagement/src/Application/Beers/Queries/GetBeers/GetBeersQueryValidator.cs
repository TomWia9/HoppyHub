﻿using FluentValidation;
using SharedUtilities.Abstractions;

namespace Application.Beers.Queries.GetBeers;

/// <summary>
///     GetBeersQuery validator.
/// </summary>
public class GetBeersQueryValidator : QueryValidator<GetBeersQuery>
{
    /// <summary>
    ///     Initializes GetBeersQueryValidator.
    /// </summary>
    public GetBeersQueryValidator()
    {
        RuleFor(x => x.Name).MaximumLength(200);
        RuleFor(x => x.MinAlcoholByVolume).InclusiveBetween(0, 100).LessThanOrEqualTo(x => x.MaxAlcoholByVolume)
            .WithMessage(MinValueMessage);
        RuleFor(x => x.MaxAlcoholByVolume).InclusiveBetween(0, 100).GreaterThanOrEqualTo(x => x.MinAlcoholByVolume)
            .WithMessage(MaxValueMessage);
        RuleFor(x => x.MinExtract).InclusiveBetween(0, 100).LessThanOrEqualTo(x => x.MaxExtract)
            .WithMessage(MinValueMessage);
        RuleFor(x => x.MaxExtract).InclusiveBetween(0, 100).GreaterThanOrEqualTo(x => x.MinExtract)
            .WithMessage(MaxValueMessage);
        RuleFor(x => x.MinIbu).InclusiveBetween(0, 200).LessThanOrEqualTo(x => x.MaxIbu)
            .WithMessage(MinValueMessage);
        RuleFor(x => x.MaxIbu).InclusiveBetween(0, 200).GreaterThanOrEqualTo(x => x.MinIbu)
            .WithMessage(MaxValueMessage);
        RuleFor(x => x.MinReleaseDate)
            .LessThanOrEqualTo(x => x.MaxReleaseDate).WithMessage(MinValueMessage);
        RuleFor(x => x.MaxReleaseDate)
            .GreaterThanOrEqualTo(x => x.MinReleaseDate).WithMessage(MaxValueMessage);
        RuleFor(x => x.MinRating).InclusiveBetween(0, 10).LessThanOrEqualTo(x => x.MaxRating)
            .WithMessage(MinValueMessage);
        RuleFor(x => x.MaxRating).InclusiveBetween(0, 10).GreaterThanOrEqualTo(x => x.MinRating)
            .WithMessage(MaxValueMessage);
        RuleFor(x => x.MinOpinionsCount).GreaterThanOrEqualTo(0).LessThanOrEqualTo(x => x.MaxOpinionsCount)
            .WithMessage(MinValueMessage);
        RuleFor(x => x.MaxOpinionsCount).GreaterThanOrEqualTo(0).GreaterThanOrEqualTo(x => x.MinOpinionsCount)
            .WithMessage(MaxValueMessage);
        RuleFor(x => x.MinFavoritesCount).GreaterThanOrEqualTo(0).LessThanOrEqualTo(x => x.MaxFavoritesCount)
            .WithMessage(MinValueMessage);
        RuleFor(x => x.MaxFavoritesCount).GreaterThanOrEqualTo(0)
            .GreaterThanOrEqualTo(x => x.MinFavoritesCount)
            .WithMessage(MaxValueMessage);
        RuleFor(x => x.SortBy)
            .Must(value =>
                string.IsNullOrWhiteSpace(value) || BeersFilteringHelper.SortingColumns.ContainsKey(value.ToUpper()))
            .WithMessage($"SortBy must be in [{string.Join(", ", BeersFilteringHelper.SortingColumns.Keys)}]");
    }
}