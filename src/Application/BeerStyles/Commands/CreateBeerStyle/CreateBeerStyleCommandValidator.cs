using Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.BeerStyles.Commands.CreateBeerStyle;

/// <summary>
///     CreateBeerStyleCommand validator.
/// </summary>
public class CreateBeerStyleCommandValidator : AbstractValidator<CreateBeerStyleCommand>
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

        RuleFor(x => x.Name).NotEmpty().MaximumLength(100).MustAsync(BeUniquelyNamed)
            .WithMessage("The beer style name must be unique.");
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.CountryOfOrigin).NotEmpty().MaximumLength(50);
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