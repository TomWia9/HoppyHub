using FluentValidation;
using SharedEvents.Events;

namespace SharedUtilities.EventValidators.Images;

/// <summary>
///     ImageDeleted event validator.
/// </summary>
public class ImageDeletedValidator : AbstractValidator<ImageDeleted>
{
    private const string InvalidImageUriErrorMessage = "Invalid image URI.";

    /// <summary>
    ///     Initializes ImageDeletedValidator.
    /// </summary>
    public ImageDeletedValidator()
    {
        RuleFor(x => x.Uri)
            .NotEmpty()
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage(InvalidImageUriErrorMessage);
    }
}