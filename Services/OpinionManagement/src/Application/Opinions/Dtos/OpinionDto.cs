using AutoMapper;
using Domain.Entities;
using SharedUtilities.Mappings;

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
    ///     The rating in 1-10 scale.
    /// </summary>
    public int Rating { get; init; }

    /// <summary>
    ///     The comment.
    /// </summary>
    public string? Comment { get; init; }

    /// <summary>
    ///     The beer image uri.
    /// </summary>
    public string? ImageUri { get; set; }

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
    public DateTimeOffset? Created { get; init; }

    /// <summary>
    ///     Date of modification.
    /// </summary>
    public DateTimeOffset? LastModified { get; init; }

    /// <summary>
    ///     Creates Opinion - OpinionDto map ignoring Username.
    /// </summary>
    /// <param name="profile">The profile</param>
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Opinion, OpinionDto>()
            .ForMember(x => x.Username, opt => opt.MapFrom(x => x.User!.Username));
    }
}