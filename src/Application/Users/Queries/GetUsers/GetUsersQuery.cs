using Application.Common.Abstractions;
using Application.Common.Models;
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
    public string? Role
    {
        get => string.IsNullOrEmpty(_role) ? _role : _role.ToLower();
        init => _role = value;
    }
    
    /// <summary>
    ///     The role.
    /// </summary>
    private readonly string? _role;
}