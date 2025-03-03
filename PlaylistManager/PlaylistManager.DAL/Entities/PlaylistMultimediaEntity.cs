namespace PlaylistManager.DAL.Entities;

public record PlaylistMultimediaEntity : IEntity
{
    public required Guid Id { get; set; }

    public required Guid PlaylistId { get; set; }
    public required Guid MultimediaId { get; set; }

    public required int Index { get; set; }

    public required PlaylistEntity Playlist { get; init; }
    public required MultimediaBaseEntity Multimedia { get; init; }
}
