using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Identity;

/// <summary>
///     The ApplicationUser class.
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
}