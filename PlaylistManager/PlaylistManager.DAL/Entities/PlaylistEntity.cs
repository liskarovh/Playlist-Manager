using PlaylistManager.Common.Enums;

namespace PlaylistManager.DAL.Entities;

/// <summary>
/// Represents the base entity for a playlist.
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
    /// Determines the type of the playlist.
    /// </summary>
    public required PlaylistType Type { get; init; }

    /// <summary>
    /// Gets or sets the collection of multimedia entities associated with the playlist.
    /// </summary>
    public ICollection<PlaylistMultimediaEntity> PlaylistMultimedia { get; set; }
        = new List<PlaylistMultimediaEntity>();
}
