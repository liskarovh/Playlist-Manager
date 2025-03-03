namespace PlaylistManager.DAL.Entities;

/// <summary>
/// Represents a playlist entity.
/// </summary>
public record PlaylistEntity : IEntity
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

    /// <summary>
    /// Gets the collection of multimedia entities associated with the playlist.
    /// </summary>
    public ICollection<PlaylistMultimediaEntity> PlaylistMultimedia { get; init; }
        = new List<PlaylistMultimediaEntity>();
}
