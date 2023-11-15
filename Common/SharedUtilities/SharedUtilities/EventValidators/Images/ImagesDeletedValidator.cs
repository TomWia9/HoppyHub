using FluentValidation;
using SharedEvents;

namespace SharedUtilities.EventValidators.Images;

/// <summary>
///     ImagesDeleted event validator.
/// </summary>
public class ImagesDeletedValidator : AbstractValidator<ImagesDeleted>
{
    /// <summary>
    ///     Initializes ImageDeletedValidator.
    /// </summary>
    public ImagesDeletedValidator()
    {
        RuleFor(x => x.Paths)
            .NotEmpty()
            .ForEach(path => path
                .NotEmpty()
                .MaximumLength(256));
    }
}