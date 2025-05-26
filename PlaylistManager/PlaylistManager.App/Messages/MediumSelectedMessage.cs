using PlaylistManager.Common.Enums;

namespace PlaylistManager.App.Messages;

public class MediumSelectedMessage
{
    public Guid SelectedPlaylistId { get; set; }
    public Guid SelectedMediumId { get; set; }
    public ManagerType ManagerType { get; set; }

    public MediumSelectedMessage(Guid playlistId, Guid mediumId, ManagerType managerType)
    {
        SelectedPlaylistId = playlistId;
        SelectedMediumId = mediumId;
        ManagerType = managerType;
    }
}
