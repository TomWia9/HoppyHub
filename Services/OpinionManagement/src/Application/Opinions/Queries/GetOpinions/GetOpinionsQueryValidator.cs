using FluentValidation;
using SharedUtilities.Abstractions;

namespace Application.Opinions.Queries.GetOpinions;

/// <summary>
///     GetOpinionsQuery validator.
/// </summary>
public class GetOpinionsQueryValidator : QueryValidator<GetOpinionsQuery>
{
    private const string IncorrectDateFormat = "Incorrect date format";
    
    /// <summary>
    ///     Initializes GetOpinionsQueryValidator.
    /// </summary>
    public GetOpinionsQueryValidator()
    {
        RuleFor(x => x.MinRating).InclusiveBetween(1, 10).LessThanOrEqualTo(x => x.MaxRating)
            .WithMessage(MinValueMessage);
        RuleFor(x => x.MaxRating).InclusiveBetween(1, 10).GreaterThanOrEqualTo(x => x.MinRating)
            .WithMessage(MaxValueMessage);
        RuleFor(x => x.From)
            .Cascade(CascadeMode.Stop)
            .Must(BeValidDate).WithMessage(IncorrectDateFormat)
            .LessThanOrEqualTo(x => x.To).WithMessage(MinValueMessage);
        RuleFor(x => x.To)
            .Cascade(CascadeMode.Stop)
            .Must(BeValidDate).WithMessage(IncorrectDateFormat)
            .GreaterThanOrEqualTo(x => x.From).WithMessage(MaxValueMessage);
        RuleFor(x => x.SortBy)
            .Must(value =>
                string.IsNullOrWhiteSpace(value) || OpinionsFilteringHelper.SortingColumns.ContainsKey(value.ToUpper()))
            .WithMessage($"SortBy must be in [{string.Join(", ", OpinionsFilteringHelper.SortingColumns.Keys)}]");
    }
    
    private bool BeValidDate(string? date)
    {
        return DateTime.TryParse(date, out _);
    }
}