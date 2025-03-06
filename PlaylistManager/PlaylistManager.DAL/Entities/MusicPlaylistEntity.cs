namespace PlaylistManager.DAL.Entities;

/// <summary>
/// Represents a music playlist entity.
/// </summary>
public record MusicPlaylistEntity : PlaylistBaseEntity
{
    /// <summary>
    /// Gets the collection of music entities associated with the playlist.
    /// </summary>
    public ICollection<MusicEntity> Multimedia { get; set; } =
        new List<MusicEntity>();
}
