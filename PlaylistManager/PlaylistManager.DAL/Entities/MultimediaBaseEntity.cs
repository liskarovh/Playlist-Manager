using PlaylistManager.Common.Enums;

namespace PlaylistManager.DAL.Entities;

public abstract record MultimediaBaseEntity : IEntity
{
    public required Guid Id { get; set; }

    public required string Title { get; set; }
    public string? Description { get; set; }

    /// <summary>
    /// In MB
    /// </summary>
    public double Size { get; set; }

    /// <summary>
    /// In seconds
    /// </summary>
    public double Duration { get; set; }
    public required string Author { get; set; } = "Unknown";
    public int? ReleasedYear { get; set; }
    public string? Url { get; set; }
}
