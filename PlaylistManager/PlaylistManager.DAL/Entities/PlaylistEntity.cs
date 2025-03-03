namespace PlaylistManager.DAL.Entities;

public record PlaylistEntity : IEntity
{
    public required Guid Id { get; set; }

    public required string Title { get; set; }
    public string? Description { get; set; }

    public int TotalMediaCount { get; set; }
    public double TotalLength { get; set; }

    public ICollection<PlaylistMultimediaEntity> PlaylistMultimedia { get; init; }
        = new List<PlaylistMultimediaEntity>();
}
