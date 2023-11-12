using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services;

/// <summary>
///     AppConfiguration class.
/// </summary>
public class AppConfiguration : IAppConfiguration
{
    /// <summary>
    ///     The configuration.
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    ///     Initializes AppConfiguration.
    /// </summary>
    /// <param name="configuration">The configuration</param>
    public AppConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    ///     Temporary beer image uri.
    /// </summary>
    public string TempBeerImageUri => _configuration.GetValue<string>("TempBeerImageUri") ??
                                      throw new InvalidOperationException("Temp beer image uri does not exists.");
}