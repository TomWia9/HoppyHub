using MediatR;

namespace Application.Users.Queries.GetUser;

/// <summary>
///     GetUser query.
/// </summary>
public record GetUserQuery : IRequest<UserDto>
{
    /// <summary>
    ///     The user id.
    /// </summary>
    public Guid UserId { get; init; }
}