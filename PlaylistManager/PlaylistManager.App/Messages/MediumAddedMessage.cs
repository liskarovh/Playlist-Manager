using CommunityToolkit.Mvvm.Messaging.Messages;
using PlaylistManager.BL.Models;

namespace PlaylistManager.App.Messages;

public class MediumAddedMessage(MediumSummaryModel medium, Guid playlistId)
    : ValueChangedMessage<MediumSummaryModel>(medium)
{
    public Guid PlaylistId { get; } = playlistId;

    public MediumSummaryModel Medium => Value;
}
