using FluentValidation;
using SharedEvents.Events;

namespace SharedUtilities.EventValidators;

/// <summary>
///     EmailRequested event validator.
/// </summary>
public class EmailRequestedValidator : AbstractValidator<EmailRequested>
{
    /// <summary>
    ///     Initializes EmailRequestedValidator.
    /// </summary>
    public EmailRequestedValidator()
    {
        RuleFor(x => x.Recipient)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Subject).NotEmpty();
        
        RuleFor(x => x.Content).NotEmpty();
    }
}