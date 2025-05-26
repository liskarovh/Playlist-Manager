using PlaylistManager.Common.Enums;

namespace PlaylistManager.App.Mappers
{
    public static class PlaylistTypeMapper
    {
        public static PlaylistType MapManagerTypeToPlaylistType(ManagerType managerType)
        {
            return managerType switch
            {
                ManagerType.Video     => PlaylistType.Video,
                ManagerType.Music     => PlaylistType.Music,
                ManagerType.AudioBook => PlaylistType.AudioBook,
                _                     => PlaylistType.Music
            };
        }

        public static ManagerType MapPlaylistTypeToManagerType(PlaylistType playlistType)
        {
            return playlistType switch
            {
                PlaylistType.Video     => ManagerType.Video,
                PlaylistType.Music     => ManagerType.Music,
                PlaylistType.AudioBook => ManagerType.AudioBook,
                _                      => ManagerType.Music
            };
        }
    }
}
