using PlaylistManager.Common.Enums;

namespace PlaylistManager.DAL.Entities;

/// <summary>
/// Represents a music entity which is a type of audio media.
/// </summary>
public record MusicEntity : AudioMediaEntity
{
    /// <summary>
    /// Gets or sets the genre of the music.
    /// </summary>
    public MusicGenre Genre { get; set; } = MusicGenre.None;
}
