using Application.Common.Abstractions;
using Application.Common.Models;
using Application.Users.Dtos;
using MediatR;

namespace Application.Users.Queries.GetUsers;

/// <summary>
///     GetUsers query.
/// </summary>
public record GetUsersQuery : QueryParameters, IRequest<PaginatedList<UserDto>> 
{
    /// <summary>
    ///     The role.
    /// </summary>
    public string? Role { get; init; }
}