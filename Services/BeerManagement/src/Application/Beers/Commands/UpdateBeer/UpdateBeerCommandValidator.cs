﻿using Application.Beers.Commands.Common;
using Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Beers.Commands.UpdateBeer;

/// <summary>
///     UpdateBeerCommand validator.
/// </summary>
public class UpdateBeerCommandValidator : BaseBeerCommandValidator<UpdateBeerCommand>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes UpdateBeerCommandValidator.
    /// </summary>
    /// <param name="context">The database context</param>
    public UpdateBeerCommandValidator(IApplicationDbContext context)
    {
        _context = context;

        RuleFor(x => x.Name)
            .MustAsync(BeUniquelyNamedWithinBrewery!)
            .WithMessage(UniqueNameErrorMessage);
    }

    /// <summary>
    ///     The custom rule indicating whether beer name is unique within brewery.
    /// </summary>
    /// <param name="model">The UpdateBeerCommand</param>
    /// <param name="name">The beer name</param>
    /// <param name="cancellationToken">The cancellation token</param>
    private async Task<bool> BeUniquelyNamedWithinBrewery(UpdateBeerCommand model, string name,
        CancellationToken cancellationToken)
    {
        return await _context.Beers.Where(x => x.Id != model.Id && x.BreweryId == model.BreweryId)
            .AllAsync(x => x.Name != name.Trim(), cancellationToken);
    }
}