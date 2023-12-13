using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Common;

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
    ///     The token secret key.
    /// </summary>
    public string JwtSecret => _configuration.GetValue<string>("JwtSettings:Secret") ??
                               throw new InvalidOperationException("JWT token secret key does not exists.");
}