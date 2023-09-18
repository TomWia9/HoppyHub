namespace Domain.Common;

/// <summary>
///     The BaseAuditableEntity class
/// </summary>
public abstract class BaseAuditableEntity
{
    /// <summary>
    ///     The entity id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    ///     The date of creation
    /// </summary>
    public DateTime? Created { get; set; }

    /// <summary>
    ///     The id of the creator
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    ///     The date of last modification
    /// </summary>
    public DateTime? LastModified { get; set; }

    /// <summary>
    ///     The id of the last modificator
    /// </summary>
    public Guid? LastModifiedBy { get; set; }
}