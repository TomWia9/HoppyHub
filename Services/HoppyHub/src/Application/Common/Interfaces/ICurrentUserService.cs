namespace Application.Common.Interfaces;

/// <summary>
///     The current user service interface.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    ///     The current user id.
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    ///     Current user role.
    /// </summary>
    string? UserRole { get; }

    /// <summary>
    ///     Indicates whether current user has administrator access.
    /// </summary>
    bool AdministratorAccess { get; }
}