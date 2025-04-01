namespace PlaylistManager.DAL.Entities;

public abstract record MultimediaBaseEntity : IEntity
{
    public required Guid Id { get; set; }

    public required string Title { get; set; }

    public string? Description { get; set; }

    public double? Duration { get; set; }

    public string? Author { get; set; }

    public int? ReleaseYear { get; set; }

    public string? Url { get; set; }
}
