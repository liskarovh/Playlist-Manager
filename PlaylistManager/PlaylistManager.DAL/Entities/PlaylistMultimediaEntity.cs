namespace PlaylistManager.DAL.Entities;

public record PlaylistMultimediaEntity : IEntity
{
    public required Guid Id { get; set; }

    public required Guid PlaylistId { get; set; }

    public required Guid MultimediaId { get; set; }

    public DateTime AddedDate { get; init; } = DateTime.Now;

    public required PlaylistEntity Playlist { get; init; }

    public required MultimediaBaseEntity Multimedia { get; init; }
}
