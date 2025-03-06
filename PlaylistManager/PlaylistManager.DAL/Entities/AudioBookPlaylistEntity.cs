namespace PlaylistManager.DAL.Entities;

/// <summary>
/// Represents an audiobook playlist entity.
/// </summary>
public record AudioBookPlaylistEntity : PlaylistBaseEntity
{
    /// <summary>
    /// Gets the collection of audiobook entities associated with the playlist.
    /// </summary>
    public ICollection<AudioBookEntity> Multimedia { get; set; } =
        new List<AudioBookEntity>();
}
