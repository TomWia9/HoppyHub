using Application.BeerStyles.Commands.Common;
using Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.BeerStyles.Commands.CreateBeerStyle;

/// <summary>
///     CreateBeerStyleCommand validator.
/// </summary>
public class CreateBeerStyleCommandValidator : BaseBeerStyleCommandValidator<CreateBeerStyleCommand>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes CreateBeerStyleCommandValidator.
    /// </summary>
    /// <param name="context">The database context</param>
    public CreateBeerStyleCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name).MustAsync(BeUniquelyNamed!)
            .WithMessage(UniqueNameErrorMessage);
    }

    /// <summary>
    ///     The custom rule indicating whether beer style name is unique.
    /// </summary>
    /// <param name="model">The CreateBeerStyleCommand</param>
    /// <param name="name">The beer style name</param>
    /// <param name="cancellationToken">The cancellation token</param>
    private async Task<bool> BeUniquelyNamed(CreateBeerStyleCommand model, string name,
        CancellationToken cancellationToken)
    {
        return await _context.BeerStyles.AllAsync(x => x.Name != name.Trim(), cancellationToken);
    }
}