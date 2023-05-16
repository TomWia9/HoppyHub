using Application.Common.Abstractions;
using FluentValidation;

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
        RuleFor(x => x.Style).MaximumLength(50);
        RuleFor(x => x.MinAlcoholByVolume).InclusiveBetween(0, 100).LessThanOrEqualTo(x => x.MaxAlcoholByVolume)
            .WithMessage("Min value must be less than or equal to Max value");
        RuleFor(x => x.MaxAlcoholByVolume).InclusiveBetween(0, 100).GreaterThanOrEqualTo(x => x.MinAlcoholByVolume)
            .WithMessage("Max value must be greater than or equal to Min value");
        RuleFor(x => x.MinExtract).InclusiveBetween(0, 100).LessThanOrEqualTo(x => x.MaxExtract)
            .WithMessage("Min value must be less than or equal to Max value");
        RuleFor(x => x.MaxExtract).InclusiveBetween(0, 100).GreaterThanOrEqualTo(x => x.MinExtract)
            .WithMessage("Max value must be greater than or equal to Min value");
        RuleFor(x => x.MinIbu).InclusiveBetween(0, 200).LessThanOrEqualTo(x => x.MaxIbu)
            .WithMessage("Min value must be less than or equal to Max value");
        RuleFor(x => x.MaxIbu).InclusiveBetween(0, 200).GreaterThanOrEqualTo(x => x.MinIbu)
            .WithMessage("Max value must be greater than or equal to Min value");
        RuleFor(x => x.SortBy)
            .Must(value =>
                string.IsNullOrWhiteSpace(value) || BeersFilteringHelper.SortingColumns.ContainsKey(value.ToUpper()))
            .WithMessage($"SortBy must be in [{string.Join(", ", BeersFilteringHelper.SortingColumns.Keys)}]");
    }
}