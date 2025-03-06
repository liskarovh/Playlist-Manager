namespace PlaylistManager.DAL.Entities;

/// <summary>
/// Represents the base entity for a playlist.
/// </summary>
public abstract record PlaylistBaseEntity : IEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the playlist entity.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the playlist.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the description of the playlist.
    /// </summary>
    public string? Description { get; set; }
}
