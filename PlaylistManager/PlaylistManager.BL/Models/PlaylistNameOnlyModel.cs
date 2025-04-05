namespace PlaylistManager.BL.Models;

public record PlaylistNameOnlyModel : ModelBase
{
    public required Guid PlaylistId { get; set; }
    public required string Title { get; set; }

    public static PlaylistNameOnlyModel Empty => new()
    {
        Id = Guid.NewGuid(),
        PlaylistId = Guid.Empty,
        Title = string.Empty
    };
}
