using FluentValidation;
using SharedEvents.Events;
using SharedUtilities.Models;

namespace SharedUtilities.EventValidators.Users;

/// <summary>
///     UserCreated event validator.
/// </summary>
public class UserCreatedValidator : AbstractValidator<UserCreated>
{
    /// <summary>
    ///     Initializes UserCreatedValidator.
    /// </summary>
    public UserCreatedValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Role)
            .Must(x => x is Roles.User or Roles.Administrator);
    }
}