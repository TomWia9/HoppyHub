using System.Text.RegularExpressions;
using Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.Breweries.Commands.UpdateBrewery;

/// <summary>
///     UpdateBreweryCommand validator.
/// </summary>
public class UpdateBreweryCommandValidator : AbstractValidator<UpdateBreweryCommand>
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
    public UpdateBreweryCommandValidator(IApplicationDbContext context, IDateTime dateTime)
    {
        _context = context;

        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.FoundationYear).NotEmpty().InclusiveBetween(0, dateTime.Now.Year);
        RuleFor(x => x.Street).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Number).NotEmpty().MaximumLength(10);
        RuleFor(x => x.PostCode).NotEmpty().Must(BeAValidPostalCode!)
            .WithMessage("Invalid postal code.");
        RuleFor(x => x.City).NotEmpty().MaximumLength(50);
        RuleFor(x => x.State).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(500)
            .MustAsync(BeUniqueName)
            .WithMessage("The brewery name must be unique.");
    }

    /// <summary>
    ///     The custom rule indicating whether brewery name is unique.
    /// </summary>
    /// <param name="model">The UpdateBreweryCommand</param>
    /// <param name="name">The brewery name</param>
    /// <param name="cancellationToken">The cancellation token</param>
    private async Task<bool> BeUniqueName(UpdateBreweryCommand model, string name,
        CancellationToken cancellationToken)
    {
        return await _context.Breweries.Where(x => x.Id != model.Id)
            .AllAsync(x => x.Name != name.Trim(), cancellationToken);
    }

    /// <summary>
    ///     The custom rule indicating whether postcode is valid.
    /// </summary>
    /// <param name="postCode">The post code</param>
    private static bool BeAValidPostalCode(string postCode)
    {
        // USA format (5 digits followed by an optional dash and 4 more digits)
        const string usaPattern = @"^\d{5}(?:[-\s]\d{4})?$";

        // Poland format (2 digits, dash, 3 digits)
        const string polandPattern = @"^\d{2}-\d{3}$";

        return Regex.IsMatch(postCode, usaPattern) || Regex.IsMatch(postCode, polandPattern);
    }
}