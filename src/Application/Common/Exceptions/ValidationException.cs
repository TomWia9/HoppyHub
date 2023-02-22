using FluentValidation.Results;

namespace Application.Common.Exceptions;

/// <summary>
///     ValidationException class.
/// </summary>
public class ValidationException : Exception
{
    /// <summary>
    ///     Initializes ValidationException.
    /// </summary>
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    /// <summary>
    ///     Initializes ValidationException with validation failures.
    /// </summary>
    /// <param name="failures"></param>
    public ValidationException(IEnumerable<ValidationFailure> failures) : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    /// <summary>
    ///     The errors.
    /// </summary>
    public IDictionary<string, string[]> Errors { get; }
}