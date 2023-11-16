using Application.BeerStyles.Commands.Common;
using Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.BeerStyles.Commands.UpdateBeerStyle;

/// <summary>
///     UpdateBeerStyleCommand validator.
/// </summary>
public class UpdateBeerStyleCommandValidator : BaseBeerStyleCommandValidator<UpdateBeerStyleCommand>
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

        RuleFor(x => x.Name).MustAsync(BeUniquelyNamed!)
            .WithMessage(UniqueNameErrorMessage);
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