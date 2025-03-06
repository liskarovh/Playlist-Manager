namespace PlaylistManager.DAL.Entities;

/// <summary>
/// Represents a video playlist entity.
/// </summary>
public record VideoPlaylistEntity : PlaylistBaseEntity
{
    /// <summary>
    /// Gets the collection of video entities associated with the playlist.
    /// </summary>
    public ICollection<VideoMediaEntity> Multimedia { get; set; } =
        new List<VideoMediaEntity>();
}
