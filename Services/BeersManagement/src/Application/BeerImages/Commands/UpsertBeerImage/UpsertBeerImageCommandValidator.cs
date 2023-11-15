using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.BeerImages.Commands.UpsertBeerImage;

/// <summary>
///     UpsertBeerImageCommand validator.
/// </summary>
public class UpsertBeerImageCommandValidator : AbstractValidator<UpsertBeerImageCommand>
{
    /// <summary>
    ///     Initializes BaseBeerImageCommandValidator.
    /// </summary>
    public UpsertBeerImageCommandValidator()
    {
        RuleFor(x => x.BeerId).NotEmpty();
        RuleFor(x => x.Image)
            
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .Must(BeAValidFile)
            .WithMessage("Only JPG and PNG files are allowed.")
            .Must(x => x!.Length <= 5 * 1024 * 1024)
            .WithMessage("The file exceeds the maximum size of 5MB.");
    }

    private bool BeAValidFile(IFormFile? file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var fileExtension = Path.GetExtension(file?.FileName);

        return allowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
    }
}