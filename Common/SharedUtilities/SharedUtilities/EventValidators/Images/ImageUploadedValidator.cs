using FluentValidation;
using SharedEvents.Responses;

namespace SharedUtilities.EventValidators.Images;

/// <summary>
///     ImageUploaded event validator.
/// </summary>
public class ImageUploadedValidator : AbstractValidator<ImageUploaded>
{
    private const string InvalidImageUriErrorMessage = "Invalid image URI.";

    /// <summary>
    ///     Initializes ImageUploadedValidator.
    /// </summary>
    public ImageUploadedValidator()
    {
        RuleFor(x => x.Uri)
            .NotEmpty()
            .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
            .WithMessage(InvalidImageUriErrorMessage);
    }
}