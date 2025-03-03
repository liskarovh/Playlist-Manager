using PlaylistManager.Common.Enums;

namespace PlaylistManager.DAL.Entities;

/// <summary>
/// Represents an audiobook entity, which is a type of audio media.
/// </summary>
public record AudioBookEntity : AudioMediaEntity
{
    /// <summary>
    /// Gets or sets the genre of the audiobook.
    /// </summary>
    public AudioBookGenre Genre { get; set; } = AudioBookGenre.None;
}
