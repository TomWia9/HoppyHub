﻿using Domain.Common;

namespace Domain.Entities;

/// <summary>
///     The beer style entity.
/// </summary>
public class BeerStyle : BaseAuditableEntity
{
    /// <summary>
    ///     The beer style name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    ///     The beer style description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    ///     The country of beer style origin.
    /// </summary>
    public string? CountryOfOrigin { get; set; }

    /// <summary>
    ///     The beers.
    /// </summary>
    public ICollection<Beer> Beers { get; set; } = new List<Beer>();
}