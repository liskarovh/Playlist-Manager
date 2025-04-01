using PlaylistManager.Common.Enums;

namespace PlaylistManager.DAL.Entities;

public record AudioMediaEntity : MultimediaBaseEntity
{
    public required AudioFormat Format { get; set; }
}
