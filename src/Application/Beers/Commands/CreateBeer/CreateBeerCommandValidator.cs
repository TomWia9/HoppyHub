using FluentValidation;

namespace Application.Beers.Commands.CreateBeer;

/// <summary>
///     CreateBeerCommand validator
/// </summary>
public class CreateBeerCommandValidator : AbstractValidator<CreateBeerCommand>
{
    /// <summary>
    ///     Initializes CreateBeerCommandValidator.
    /// </summary>
    public CreateBeerCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Brewery).NotEmpty().MaximumLength(200);
        RuleFor(x => x.AlcoholByVolume).NotNull().InclusiveBetween(0, 100);
        RuleFor(x => x.Description).MaximumLength(3000);
        RuleFor(x => x.SpecificGravity).InclusiveBetween(0, 1.2);
        RuleFor(x => x.Blg).InclusiveBetween(0, 100);
        RuleFor(x => x.Plato).InclusiveBetween(0, 100);
        RuleFor(x => x.Style).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Ibu).InclusiveBetween(0, 200);
        RuleFor(x => x.Country).NotEmpty().MaximumLength(50);
    }
}