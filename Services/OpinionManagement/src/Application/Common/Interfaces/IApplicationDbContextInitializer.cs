namespace Application.Common.Interfaces;

public interface IApplicationDbContextInitializer
{
    /// <summary>
    ///     Initializes database asynchronously.
    /// </summary>
    public Task InitializeAsync();
}