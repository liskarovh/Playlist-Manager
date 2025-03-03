using PlaylistManager.Common.Enums;

namespace PlaylistManager.DAL.Entities;

public record AudioBookEntity : AudioMediaEntity
{
    public AudioBookGenre Genre { get; set; }
}