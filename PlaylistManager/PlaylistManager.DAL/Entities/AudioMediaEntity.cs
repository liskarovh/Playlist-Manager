using PlaylistManager.Common.Enums;

namespace PlaylistManager.DAL.Entities;

/// <summary>
/// Represents an audio media entity, which is a type of multimedia base entity.
/// </summary>
public record AudioMediaEntity : MultimediaBaseEntity
{
    /// <summary>
    /// Gets or sets the format of the audio media.
    /// </summary>
    public required AudioFormat Format { get; set; }
}
