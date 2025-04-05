using PlaylistManager.Common.Enums;

namespace PlaylistManager.BL.Models;

public record PlaylistSummaryModel : ModelBase
{
    public required Guid PlaylistId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public double? TotalDuration { get; set; }
    public uint MediaCount { get; set; }
    public PlaylistType? PlaylistType { get; set; }

    public static PlaylistSummaryModel Empty => new()
    {
        Id = Guid.NewGuid(),
        PlaylistId = Guid.Empty,
        Title = string.Empty,
        MediaCount = 0
    };
}
