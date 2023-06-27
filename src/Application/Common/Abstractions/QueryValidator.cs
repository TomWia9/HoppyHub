using FluentValidation;

namespace Application.Common.Abstractions;

/// <summary>
///     Abstract query validator for query parameters.
/// </summary>
public abstract class QueryValidator<T> : AbstractValidator<T> where T : QueryParameters
{
    protected const string MinValueMessage = "Min value must be less than or equal to Max value";
    protected const string MaxValueMessage = "Max value must be greater than or equal to Min value";

    /// <summary>
    ///     Initializes QueryValidator.
    /// </summary>
    protected QueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1);
        RuleFor(x => x.SearchQuery).MaximumLength(100);
        RuleFor(x => x.SortDirection).IsInEnum();
        RuleFor(x => x.SortBy).MaximumLength(50);
    }
}