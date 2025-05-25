using CommunityToolkit.Mvvm.Messaging.Messages;

namespace PlaylistManager.App.Messages;

public class MediumRemovedMessage(Guid mediumId)
    : ValueChangedMessage<Guid>(mediumId)
{
    public Guid MediumId => Value;
}
