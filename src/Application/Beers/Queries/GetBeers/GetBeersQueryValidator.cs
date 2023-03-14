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
        RuleFor(x => x.Brewery).MaximumLength(200);
        RuleFor(x => x.Style).MaximumLength(50);
        RuleFor(x => x.Country).MaximumLength(50);
        RuleFor(x => x.MinAlcoholByVolume).InclusiveBetween(0, 100);
        RuleFor(x => x.MaxAlcoholByVolume).InclusiveBetween(0, 100);
        RuleFor(x => x.MinSpecificGravity).InclusiveBetween(0, 1.2);
        RuleFor(x => x.MaxSpecificGravity).InclusiveBetween(0, 1.2);
        RuleFor(x => x.MinBlg).InclusiveBetween(0, 100);
        RuleFor(x => x.MaxBlg).InclusiveBetween(0, 100);
        RuleFor(x => x.MinPlato).InclusiveBetween(0, 100);
        RuleFor(x => x.MaxPlato).InclusiveBetween(0, 100);
        RuleFor(x => x.MinIbu).InclusiveBetween(0, 200);
        RuleFor(x => x.MaxIbu).InclusiveBetween(0, 200);
        RuleFor(x => x.SortBy)
            .Must(value =>
                string.IsNullOrWhiteSpace(value) || BeersFilteringHelper.SortingColumns.ContainsKey(value.ToUpper()))
            .WithMessage($"SortBy must be in [{string.Join(", ", BeersFilteringHelper.SortingColumns.Keys)}]");
    }
}