using PlaylistManager.Common.Enums;
namespace PlaylistManager.App.Messages;

public record ManagerSelectedMessage
{
    public required ManagerType SelectedType { get; init; }
}
