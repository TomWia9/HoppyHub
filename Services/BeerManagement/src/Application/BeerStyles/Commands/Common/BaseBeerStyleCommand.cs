namespace Application.BeerStyles.Commands.Common;

/// <summary>
///     BaseBeerStyleCommand abstract record.
/// </summary>
public abstract record BaseBeerStyleCommand
{
    /// <summary>
    ///     The beer style name.
    /// </summary>
    public string? Name { get; init; }

    /// <summary>
    ///     The beer style description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    ///     The country of beer style origin.
    /// </summary>
    public string? CountryOfOrigin { get; init; }
}