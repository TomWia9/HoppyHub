using Application.Common.Mappings;
using Domain.Entities;

namespace Application.Opinions.Dtos;

/// <summary>
///     The opinion data transfer object.
/// </summary>
public record OpinionDto : IMapFrom<Opinion>
{
    /// <summary>
    ///     The opinion id.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    ///     The rate in 1-10 scale.
    /// </summary>
    public int Rate { get; init; }

    /// <summary>
    ///     The comment.
    /// </summary>
    public string? Comment { get; init; }

    /// <summary>
    ///     The beer id.
    /// </summary>
    public Guid BeerId { get; init; }

    /// <summary>
    ///     The user id.
    /// </summary>
    public Guid? CreatedBy { get; init; }

    /// <summary>
    ///     The username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    ///     Date of creation.
    /// </summary>
    public DateTime? Created { get; init; }

    /// <summary>
    ///     Date of modification.
    /// </summary>
    public DateTime? LastModified { get; init; }
}