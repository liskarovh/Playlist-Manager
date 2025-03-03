using PlaylistManager.Common.Enums;

namespace PlaylistManager.DAL.Entities;

/// <summary>
/// Represents a video media entity which is a type of multimedia base entity.
/// </summary>
public record VideoMediaEntity : MultimediaBaseEntity
{
    /// <summary>
    /// Gets or sets the format of the video media.
    /// </summary>
    public required VideoFormat Format { get; set; }

    /// <summary>
    /// Gets or sets the genre of the video.
    /// </summary>
    public VideoGenre Genre { get; set; } = VideoGenre.None;

    /// <summary>
    /// Gets or sets the resolution of the video.
    /// </summary>
    public string? Resolution { get; set; }
}
