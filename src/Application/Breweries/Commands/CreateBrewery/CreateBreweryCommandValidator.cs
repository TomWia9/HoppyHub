using Application.Breweries.Commands.Common;
using Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Breweries.Commands.CreateBrewery;

/// <summary>
///     CreateBreweryCommand validator
/// </summary>
public class CreateBreweryCommandValidator : BaseBreweryCommandValidator<CreateBreweryCommand>
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
    public CreateBreweryCommandValidator(IApplicationDbContext context, IDateTime dateTime) : base(dateTime)
    {
        _context = context;

        RuleFor(x => x.Name).MustAsync(BeUniquelyNamed!).WithMessage(UniqueNameErrorMessage);
    }

    /// <summary>
    ///     The custom rule indicating whether brewery name is unique.
    /// </summary>
    /// <param name="model">The CreateBreweryCommand</param>
    /// <param name="name">The brewery name</param>
    /// <param name="cancellationToken">The cancellation token</param>
    private async Task<bool> BeUniquelyNamed(CreateBreweryCommand model, string name,
        CancellationToken cancellationToken)
    {
        return await _context.Breweries.AllAsync(x => x.Name != name.Trim(), cancellationToken);
    }
}