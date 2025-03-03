using PlaylistManager.Common.Enums;

namespace PlaylistManager.DAL.Entities;

public record MusicEntity : AudioMediaEntity
{
    public MusicGenre Genre { get; set; }
}