using FluentValidation;
using SharedEvents;

namespace SharedUtilities.EventValidators.Users;

/// <summary>
///     UserUpdated event validator.
/// </summary>
public class UserUpdatedValidator : AbstractValidator<UserUpdated>
{
    /// <summary>
    ///     Initializes UserUpdatedValidator.
    /// </summary>
    public UserUpdatedValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(256);
    }
}