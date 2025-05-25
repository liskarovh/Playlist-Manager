using PlaylistManager.Common.Enums;
using PlaylistManager.BL.Models;

namespace PlaylistManager.App.Messages;

public class PlaylistDisplayMessage
{
    public Guid SelectedPlaylistId { get; set; }

    public ManagerType ManagerType { get; set; }

    public PlaylistDisplayMessage(Guid playlistId, ManagerType managerType)
    {
        SelectedPlaylistId = playlistId;
        ManagerType = managerType;
    }
}
