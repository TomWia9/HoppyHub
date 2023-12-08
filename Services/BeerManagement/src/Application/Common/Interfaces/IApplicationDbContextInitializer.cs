namespace Application.Common.Interfaces;

/// <summary>
///     The ApplicationDbContextInitializer interface.
/// </summary>
public interface IApplicationDbContextInitializer
{
    /// <summary>
    ///     Initializes database asynchronously.
    /// </summary>
    public Task InitializeAsync();
}