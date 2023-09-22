using Application.Common.Interfaces;
using FluentValidation;
using SharedUtilities.Abstractions;

namespace Application.Breweries.Queries.GetBreweries;

/// <summary>
///     GetBreweriesQuery validator.
/// </summary>
public class GetBreweriesQueryValidator : QueryValidator<GetBreweriesQuery>
{
    /// <summary>
    ///     Initializes GetBreweriesQueryValidator.
    /// </summary>
    public GetBreweriesQueryValidator(IDateTime dateTime)
    {
        RuleFor(x => x.Name).MaximumLength(500);
        RuleFor(x => x.Country).MaximumLength(50);
        RuleFor(x => x.State).MaximumLength(50);
        RuleFor(x => x.City).MaximumLength(50);
        RuleFor(x => x.MinFoundationYear).InclusiveBetween(0, dateTime.Now.Year)
            .LessThanOrEqualTo(x => x.MaxFoundationYear)
            .WithMessage(MinValueMessage);
        RuleFor(x => x.MaxFoundationYear).InclusiveBetween(0, dateTime.Now.Year)
            .GreaterThanOrEqualTo(x => x.MinFoundationYear)
            .WithMessage(MaxValueMessage);
        RuleFor(x => x.SortBy)
            .Must(value =>
                string.IsNullOrWhiteSpace(value) ||
                BreweriesFilteringHelper.SortingColumns.ContainsKey(value.ToUpper()))
            .WithMessage($"SortBy must be in [{string.Join(", ", BreweriesFilteringHelper.SortingColumns.Keys)}]");
    }
}