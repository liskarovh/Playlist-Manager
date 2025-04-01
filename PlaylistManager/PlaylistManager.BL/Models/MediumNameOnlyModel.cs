namespace PlaylistManager.BL.Models;

public record MediumNameOnlyModel : ModelBase
{
    public required Guid MediumId { get; set; }
    public required string Title { get; set; }
    public required DateTime AddedDate { get; set; }

    public static MediumNameOnlyModel Empty => new()
    {
        Id = Guid.NewGuid(),
        MediumId = Guid.Empty,
        Title = string.Empty,
        AddedDate = DateTime.MinValue
    };
}
