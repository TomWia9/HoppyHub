using Application.Users.Dtos;
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
    public Guid Id { get; init; }
}