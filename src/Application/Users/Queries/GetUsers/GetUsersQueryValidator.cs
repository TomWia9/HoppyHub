using Application.Common.Abstractions;
using FluentValidation;

namespace Application.Users.Queries.GetUsers;

/// <summary>
///     GetUsersQuery validator.
/// </summary>
public class GetUsersQueryValidator : QueryValidator<GetUsersQuery>
{
    /// <summary>
    ///     The columns allowed to sort by.
    /// </summary>
    private readonly IEnumerable<string> _sortingColumns = new List<string>
    {
        "EMAIL",
        "USERNAME"
    };

    /// <summary>
    ///     Initializes GetUsersQueryValidator.
    /// </summary>
    public GetUsersQueryValidator()
    {
        RuleFor(x => x.Role)
            .MaximumLength(15);

        RuleFor(x => x.SortBy)
            .Must(value =>
                string.IsNullOrWhiteSpace(value) || _sortingColumns.Contains(value.ToUpper()))
            .WithMessage($"SortBy must be in [{string.Join(", ", _sortingColumns)}]");
    }
}