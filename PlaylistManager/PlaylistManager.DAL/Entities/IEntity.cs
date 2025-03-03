namespace PlaylistManager.DAL.Entities;

/// <summary>
/// Represents a generic entity with an Guid identifier.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    Guid Id { get; set; }
}
