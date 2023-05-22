using Application.Common.Abstractions;
using FluentValidation;

namespace Application.BeerStyles.Queries.GetBeerStyles;

/// <summary>
///     GetBeerStylesQuery validator.
/// </summary>
public class GetBeerStylesQueryValidator : QueryValidator<GetBeerStylesQuery>
{
    /// <summary>
    ///     Initializes GetBeerStylesQueryValidator.
    /// </summary>
    public GetBeerStylesQueryValidator()
    {
        RuleFor(x => x.CountryOfOrigin).MaximumLength(50);
    }
}