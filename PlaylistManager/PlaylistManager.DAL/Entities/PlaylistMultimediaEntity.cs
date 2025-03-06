namespace PlaylistManager.DAL.Entities;

/// <summary>
/// Represents a multimedia entity within a playlist.
/// </summary>
public record PlaylistMultimediaEntity : IEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the playlist multimedia entity.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the associated playlist.
    /// </summary>
    public required Guid PlaylistId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the associated multimedia.
    /// </summary>
    public required Guid MultimediaId { get; set; }

    /// <summary>
    /// Gets the associated playlist entity.
    /// </summary>
    public required PlaylistBaseEntity Playlist { get; init; }

    /// <summary>
    /// Gets the associated multimedia entity.
    /// </summary>
    public required MultimediaBaseEntity Multimedia { get; init; }
}
