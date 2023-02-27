using Application.Abstractions;
using MediatR;

namespace Application.Users.Queries.GetUsers;

/// <summary>
///     GetUsers query.
/// </summary>
public record GetUsersQuery : QueryParameters, IRequest<IEnumerable<UserDto>> 
{
    public string? Role
    {
        get => string.IsNullOrEmpty(_role) ? _role : _role.ToLower();
        init => _role = value;
    }
    
    private readonly string? _role;
}