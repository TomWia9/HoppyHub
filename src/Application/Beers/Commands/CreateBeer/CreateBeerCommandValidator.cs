using Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Beers.Commands.CreateBeer;

/// <summary>
///     CreateBeerCommand validator
/// </summary>
public class CreateBeerCommandValidator : AbstractValidator<CreateBeerCommand>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes CreateBeerCommandValidator.
    /// </summary>
    /// <param name="context">The database context</param>
    public CreateBeerCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Brewery).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AlcoholByVolume).NotNull().InclusiveBetween(0, 100);
        RuleFor(x => x.Description).MaximumLength(3000);
        RuleFor(x => x.Blg).InclusiveBetween(0, 100);
        RuleFor(x => x.Plato).InclusiveBetween(0, 100);
        RuleFor(x => x.Style).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Ibu).InclusiveBetween(0, 200);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200)
            .MustAsync(BeUniqueNameWithinBrewery)
            .WithMessage("The beer name must be unique within the brewery.");
    }

    /// <summary>
    ///     The custom rule indicating whether beer name is unique within brewery.
    /// </summary>
    /// <param name="model">The CreateBeerCommand</param>
    /// <param name="name">The beer name</param>
    /// <param name="cancellationToken">The cancellation token</param>
    private async Task<bool> BeUniqueNameWithinBrewery(CreateBeerCommand model, string name,
        CancellationToken cancellationToken)
    {
        return await _context.Beers.Where(x => x.Brewery == model.Brewery)
            .AllAsync(x => x.Name != name.Trim(), cancellationToken);
    }
}