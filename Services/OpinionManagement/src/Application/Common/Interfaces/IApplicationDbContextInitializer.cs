namespace Application.Common.Interfaces;

/// <summary>
///     ApplicationDbContextInitializer interface.
/// </summary>
public interface IApplicationDbContextInitializer
{
    /// <summary>
    ///     Initializes database asynchronously.
    /// </summary>
    public Task InitializeAsync();
}