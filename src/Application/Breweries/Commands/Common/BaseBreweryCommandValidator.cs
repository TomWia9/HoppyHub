using System.Text.RegularExpressions;
using Application.Common.Interfaces;
using FluentValidation;

namespace Application.Breweries.Commands.Common;

/// <summary>
///     BaseBreweryCommand abstract validator.
/// </summary>
public abstract class BaseBreweryCommandValidator<TCommand> : AbstractValidator<TCommand>
    where TCommand : BaseBreweryCommand
{
    protected const string UniqueNameErrorMessage = "The brewery name must be unique.";
    private const string InvalidUrlErrorMessage = "Invalid URL.";
    private const string InvalidPostalCodeErrorMessage = "Invalid postal code.";

    /// <summary>
    ///     Initializes BaseBreweryCommandValidator.
    /// </summary>
    protected BaseBreweryCommandValidator(IDateTime dateTime)
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.FoundationYear).NotEmpty().InclusiveBetween(0, dateTime.Now.Year);
        RuleFor(x => x.WebsiteUrl).MaximumLength(200).Must(url => Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage(InvalidUrlErrorMessage);
        RuleFor(x => x.Street).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Number).NotEmpty().MaximumLength(10);
        RuleFor(x => x.PostCode).NotEmpty().Must(BeAValidPostalCode)
            .WithMessage(InvalidPostalCodeErrorMessage);
        RuleFor(x => x.City).NotEmpty().MaximumLength(50);
        RuleFor(x => x.State).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(500);
    }

    /// <summary>
    ///     The custom rule indicating whether postcode is valid.
    /// </summary>
    /// <param name="postCode">The post code</param>
    private static bool BeAValidPostalCode(string? postCode)
    {
        // USA format (5 digits followed by an optional dash and 4 more digits)
        const string usaPattern = @"^\d{5}(?:[-\s]\d{4})?$";

        // Poland format (2 digits, dash, 3 digits)
        const string polandPattern = @"^\d{2}-\d{3}$";

        return !string.IsNullOrEmpty(postCode) &&
               (Regex.IsMatch(postCode, usaPattern, RegexOptions.NonBacktracking) ||
                Regex.IsMatch(postCode, polandPattern, RegexOptions.NonBacktracking));
    }
}