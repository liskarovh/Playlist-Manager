namespace PlaylistManager.DAL.Entities;

/// <summary>
/// Represents the base entity for multimedia content.
/// </summary>
public abstract record MultimediaBaseEntity : IEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the multimedia entity.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the title of the multimedia content.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Gets or sets the description of the multimedia content.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the duration of the multimedia content in seconds.
    /// </summary>
    public double? Duration { get; set; }

    /// <summary>
    /// Gets or sets the author of the multimedia content.
    /// </summary>
    public string? Author { get; set; }

    /// <summary>
    /// Gets or sets the year the multimedia content released.
    /// </summary>
    public int? ReleaseYear { get; set; }

    /// <summary>
    /// Gets or sets the URL of the multimedia content.
    /// </summary>
    public string? Url { get; set; }
}
