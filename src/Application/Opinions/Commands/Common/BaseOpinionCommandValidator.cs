using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Application.Opinions.Commands.Common;

/// <summary>
///     BaseOpinionCommand abstract validator.
/// </summary>
public abstract class BaseOpinionCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : BaseOpinionCommand
{
    /// <summary>
    ///     Initializes BaseOpinionCommandValidator.
    /// </summary>
    protected BaseOpinionCommandValidator()
    {
        RuleFor(x => x.Rating).NotEmpty().InclusiveBetween(1, 10);
        RuleFor(x => x.Comment).MaximumLength(1000);
        RuleFor(x => x.Image)
            .Must(BeAValidFile).When(x => x.Image != null)
            .WithMessage("Only JPG and PNG files are allowed.")
            .Must(x => x == null || x.Length <= 5 * 1024 * 1024)
            .WithMessage("The file exceeds the maximum size of 5MB.");
    }

    private bool BeAValidFile(IFormFile? file)
    {
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var fileExtension = Path.GetExtension(file?.FileName);

        return allowedExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase);
    }
}