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
        RuleFor(x => x.MinRating).InclusiveBetween(1, 10).LessThanOrEqualTo(x => x.MaxRating)
            .WithMessage(MinValueMessage);
        RuleFor(x => x.MaxRating).InclusiveBetween(1, 10).GreaterThanOrEqualTo(x => x.MinRating)
            .WithMessage(MaxValueMessage);
        RuleFor(x => x.SortBy)
            .Must(value =>
                string.IsNullOrWhiteSpace(value) || OpinionsFilteringHelper.SortingColumns.ContainsKey(value.ToUpper()))
            .WithMessage($"SortBy must be in [{string.Join(", ", OpinionsFilteringHelper.SortingColumns.Keys)}]");
    }
}