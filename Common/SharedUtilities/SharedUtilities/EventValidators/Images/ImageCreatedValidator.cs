using FluentValidation;
using SharedEvents;

namespace SharedUtilities.EventValidators.Images;

/// <summary>
///     ImageCreated event validator.
/// </summary>
public class ImageCreatedValidator : AbstractValidator<ImageCreated>
{
    /// <summary>
    ///     Initializes ImageCreatedValidator.
    /// </summary>
    public ImageCreatedValidator()
    {
        RuleFor(x => x.Path)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.Image)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(x => x!.Length <= 5 * 1024 * 1024)
            .WithMessage("The file exceeds the maximum size of 5MB.");
    }
}