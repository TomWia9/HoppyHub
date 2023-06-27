using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.Persistence.Extensions;

/// <summary>
///     Entity entry extensions class
/// </summary>
public static class EntityEntryExtensions
{
    /// <summary>
    ///     Checks if entry has changed owned entities
    /// </summary>
    /// <param name="entry">The entry</param>
    public static bool HasChangedOwnedEntities(this EntityEntry entry)
    {
        return entry.References.Any(r =>
            r.TargetEntry is not null &&
            r.TargetEntry.Metadata.IsOwned() &&
            r.TargetEntry.State is EntityState.Added or EntityState.Modified);
    }
}