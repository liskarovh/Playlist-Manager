using CommunityToolkit.Mvvm.Messaging.Messages;

namespace PlaylistManager.App.Messages;

public class PlaylistDeleteMessage(string playlistId)
    : ValueChangedMessage<string>(playlistId);
