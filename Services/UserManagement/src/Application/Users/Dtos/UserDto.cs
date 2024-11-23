namespace Application.Users.Dtos;

/// <summary>
///     UserDto record.
/// </summary>
public record UserDto
{
    /// <summary>
    ///     The user id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     The email.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    ///     The username.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    ///     The role.
    /// </summary>
    public string? Role { get; set; }
    
    /// <summary>
    ///     Date of account creation.
    /// </summary>
    public DateTimeOffset? Created { get; init; }
}