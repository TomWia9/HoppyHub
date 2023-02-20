namespace Application.Common.Interfaces;

/// <summary>
///     The current user service interface
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    ///     Current user id
    /// </summary>
    Guid? UserId { get; }
}