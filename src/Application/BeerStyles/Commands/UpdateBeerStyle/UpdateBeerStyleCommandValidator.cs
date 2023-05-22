using Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.BeerStyles.Commands.UpdateBeerStyle;

/// <summary>
///     UpdateBeerStyleCommand validator.
/// </summary>
public class UpdateBeerStyleCommandValidator : AbstractValidator<UpdateBeerStyleCommand>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes UpdateBeerStyleCommandValidator.
    /// </summary>
    /// <param name="context">The database context</param>
    public UpdateBeerStyleCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name).NotEmpty().MaximumLength(100).MustAsync(BeUniquelyNamed)
            .WithMessage("The beer style name must be unique.");
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.CountryOfOrigin).NotEmpty().MaximumLength(50);
    }

    /// <summary>
    ///     The custom rule indicating whether beer style name is unique.
    /// </summary>
    /// <param name="model">The UpdateBeerStyleCommand</param>
    /// <param name="name">The beer style name</param>
    /// <param name="cancellationToken">The cancellation token</param>
    private async Task<bool> BeUniquelyNamed(UpdateBeerStyleCommand model, string name,
        CancellationToken cancellationToken)
    {
        return await _context.BeerStyles.Where(x => x.Id != model.Id)
            .AllAsync(x => x.Name != name.Trim(), cancellationToken);
    }
}