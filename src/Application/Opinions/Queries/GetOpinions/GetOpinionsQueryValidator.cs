using Application.Common.Abstractions;
using FluentValidation;

namespace Application.Opinions.Queries.GetOpinions;

/// <summary>
///     GetOpinionsQuery validator.
/// </summary>
public class GetOpinionsQueryValidator : QueryValidator<GetOpinionsQuery>
{
    /// <summary>
    ///     Initializes GetOpinionsQueryValidator.
    /// </summary>
    public GetOpinionsQueryValidator()
    {
        RuleFor(x => x.MinRate).InclusiveBetween(1, 10).LessThanOrEqualTo(x => x.MaxRate)
            .WithMessage("Min value must be less than or equal to Max value");
        RuleFor(x => x.MaxRate).InclusiveBetween(1, 10).GreaterThanOrEqualTo(x => x.MinRate)
            .WithMessage("Max value must be greater than or equal to Min value");
        RuleFor(x => x.SortBy)
            .Must(value =>
                string.IsNullOrWhiteSpace(value) || OpinionsFilteringHelper.SortingColumns.ContainsKey(value.ToUpper()))
            .WithMessage($"SortBy must be in [{string.Join(", ", OpinionsFilteringHelper.SortingColumns.Keys)}]");
    }
}