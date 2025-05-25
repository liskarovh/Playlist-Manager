using CommunityToolkit.Mvvm.Messaging.Messages;
using PlaylistManager.BL.Models;

namespace PlaylistManager.App.Messages;

public class MediumSelectedMessage(MediumSummaryModel medium)
    : ValueChangedMessage<MediumSummaryModel>(medium)
{
    public MediumSummaryModel Medium => Value;
}
