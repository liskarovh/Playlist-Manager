namespace PlaylistManager.BL.Models;

public record MediumDetailedModel : ModelBase
{
    public required Guid MediumId { get; set; }
    public required Guid PlaylistId { get; set; }
    public required string Title { get; set; }
    public string? Author  { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public double? Duration { get; set; }
    public int? ReleaseYear { get; set; }
    public required string Format { get; set; }
    public required string Genre { get; set; }
    public DateTime AddedDate { get; set; }

    public static MediumDetailedModel Empty => new()
    {
        Id = Guid.NewGuid(),
        MediumId = Guid.Empty,
        PlaylistId = Guid.Empty,
        Title = string.Empty,
        AddedDate = DateTime.MinValue,
        Format = string.Empty,
        Genre = string.Empty,
    };
}
