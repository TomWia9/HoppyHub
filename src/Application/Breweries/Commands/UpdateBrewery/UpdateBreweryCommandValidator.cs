using Application.Breweries.Commands.Common;
using Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Breweries.Commands.UpdateBrewery;

/// <summary>
///     UpdateBreweryCommand validator.
/// </summary>
public class UpdateBreweryCommandValidator : BaseBreweryCommandValidator<UpdateBreweryCommand>
{
    /// <summary>
    ///     The database context.
    /// </summary>
    private readonly IApplicationDbContext _context;

    /// <summary>
    ///     Initializes CreateBreweryCommandValidator.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="dateTime">The date time service</param>
    public UpdateBreweryCommandValidator(IApplicationDbContext context, IDateTime dateTime) : base(dateTime)
    {
        _context = context;

        RuleFor(x => x.Name).MustAsync(BeUniquelyNamed!).WithMessage(UniqueNameErrorMessage);
    }

    /// <summary>
    ///     The custom rule indicating whether brewery name is unique.
    /// </summary>
    /// <param name="model">The UpdateBreweryCommand</param>
    /// <param name="name">The brewery name</param>
    /// <param name="cancellationToken">The cancellation token</param>
    private async Task<bool> BeUniquelyNamed(UpdateBreweryCommand model, string name,
        CancellationToken cancellationToken)
    {
        return await _context.Breweries.Where(x => x.Id != model.Id)
            .AllAsync(x => x.Name != name.Trim(), cancellationToken);
    }
}