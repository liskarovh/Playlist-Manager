using CommunityToolkit.Mvvm.Messaging.Messages;
using PlaylistManager.BL.Models;

namespace PlaylistManager.App.Messages;

public class PlaylistSelectedMessage(PlaylistSummaryModel playlist)
    : ValueChangedMessage<PlaylistSummaryModel>(playlist);
