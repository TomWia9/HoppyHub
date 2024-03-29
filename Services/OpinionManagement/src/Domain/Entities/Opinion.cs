﻿using Domain.Common;

namespace Domain.Entities;

/// <summary>
///     The opinion entity class.
/// </summary>
public class Opinion : BaseAuditableEntity
{
    /// <summary>
    ///     The rating in 1-10 scale.
    /// </summary>
    public int Rating { get; set; }

    /// <summary>
    ///     The comment.
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    ///     The beer image uri.
    /// </summary>
    public string? ImageUri { get; set; }

    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid BeerId { get; set; }

    /// <summary>
    ///     The beer.
    /// </summary>
    public Beer? Beer { get; set; }

    /// <summary>
    ///     The user.
    /// </summary>
    public User? User { get; set; }
}