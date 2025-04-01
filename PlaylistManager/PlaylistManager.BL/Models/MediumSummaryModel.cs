namespace PlaylistManager.BL.Models;

public record MediumSummaryModel :ModelBase
{
    public required Guid MediumId { get; set; }
    public required string Title { get; set; }
    public string? Author  { get; set; }
    public double? Duration { get; set; }
    public required DateTime AddedDate { get; set; }

    public static MediumSummaryModel Empty => new()
    {
        Id = Guid.NewGuid(),
        MediumId = Guid.Empty,
        Title = string.Empty,
        AddedDate = DateTime.MinValue
    };
}
