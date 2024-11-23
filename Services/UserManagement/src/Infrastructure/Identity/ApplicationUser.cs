using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

/// <summary>
///     The ApplicationUser class.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>
    ///     The date of account creation.
    /// </summary>
    public DateTimeOffset Created { get; set; }
}