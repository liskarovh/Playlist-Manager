using PlaylistManager.Common.Enums;

namespace PlaylistManager.DAL.Entities;

public record PlaylistEntity : IEntity
{
    public required Guid Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public required PlaylistType Type { get; init; }

    public ICollection<PlaylistMultimediaEntity> PlaylistMultimedia { get; set; }
        = new List<PlaylistMultimediaEntity>();
}
