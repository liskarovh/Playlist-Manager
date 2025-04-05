namespace PlaylistManager.BL.Models;

public record MediumDetailedModel : ModelBase
{
    public required Guid MediumId { get; set; }
    public required string Title { get; set; }
    public string? Author  { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public double? Duration { get; set; }
    public int? ReleaseYear { get; set; }
    public  string? Format { get; set; }
    public string? Genre { get; set; }
    public DateTime AddedDate { get; set; }

    public static MediumDetailedModel Empty => new()
    {
        Id = Guid.NewGuid(),
        MediumId = Guid.Empty,
        Title = string.Empty,
        AddedDate = DateTime.Now
    };
}
