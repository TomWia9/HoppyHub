using FluentValidation;

namespace Application.Abstractions;

/// <summary>
///     Abstract query validator for query parameters.
/// </summary>
public abstract class QueryValidator<T> : AbstractValidator<T> where T : QueryParameters
{
    /// <summary>
    ///     Initializes QueryValidator.
    /// </summary>
    protected QueryValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1);
        RuleFor(x => x.SearchQuery).MaximumLength(100);
        RuleFor(x => x.SortDirection).IsInEnum();
    }
}