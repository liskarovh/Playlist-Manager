using PlaylistManager.Common.Enums;

namespace PlaylistManager.DAL.Entities;

public record VideoMediaEntity : MultimediaBaseEntity
{
    public required VideoFormat Format { get; set; }
    public VideoGenre Genre { get; set; }
    public string? Resolution { get; set; }
}
