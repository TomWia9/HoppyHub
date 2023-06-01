using FluentValidation;

namespace Application.Opinions.Commands.UpdateOpinion;

/// <summary>
///     UpdateOpinionCommand validator.
/// </summary>
public class UpdateOpinionCommandValidator : AbstractValidator<UpdateOpinionCommand>
{
    /// <summary>
    ///     Initializes UpdateOpinionCommandValidator.
    /// </summary>
    public UpdateOpinionCommandValidator()
    {
        RuleFor(x => x.Rating).NotEmpty().InclusiveBetween(1, 10);
        RuleFor(x => x.Comment).MaximumLength(1000);
    }
}