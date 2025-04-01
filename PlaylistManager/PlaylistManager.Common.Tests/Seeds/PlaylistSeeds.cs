using Microsoft.EntityFrameworkCore;
using PlaylistManager.DAL.Entities;
using PlaylistManager.Common.Enums;

namespace PlaylistManager.Common.Tests.Seeds;

public static class PlaylistSeeds
{
    public static readonly PlaylistEntity EmptyPlaylist = new()
    {
        Id = default,
        Title = default!,
        Description = default,
        Type = default!,
        PlaylistMultimedia = new List<PlaylistMultimediaEntity>()
    };

    public static readonly PlaylistEntity MusicPlaylist = new()
    {
        Id = Guid.Parse(input: "dfcdc163-a6fb-4edb-80b9-1de8f1461a85"),
        Title = "Rock Playlist",
        Description = "A collection of rock songs",
        Type = PlaylistType.Music
    };

    public static readonly PlaylistEntity VideoPlaylist = new()
    {
        Id = Guid.Parse(input: "78f505ba-ba72-4d84-8e8d-a08e29182192"),
        Title = "Favorite Movies",
        Description = "List of favorite sci-fi movies",
        Type = PlaylistType.Video
    };

    public static readonly PlaylistEntity AudioBookPlaylist = new()
    {
        Id = Guid.Parse(input: "341f5161-841e-498d-974c-a2cea40a3c79"),
        Title = "Fantasy Audiobooks",
        Description = "Audiobook journey through fantasy worlds",
        Type = PlaylistType.AudioBook
    };

    public static readonly PlaylistEntity MusicPlaylistForMultimediaUpdate = new()
    {
        Id = Guid.Parse(input: "41b93013-7605-4ee1-bbfb-e1503f125825"),
        Title = "Rock Playlist Updated",
        Description = "Updated best of rock 2025",
        Type = PlaylistType.Music,
    };

    public static readonly PlaylistEntity MusicPlaylistUpdate = new()
    {
        Id = Guid.Parse(input: "42b93013-7605-4ee1-bbfb-e1503f125826"),
        Title = "Rock Playlist Updated",
        Description = "Updated best of rock 2025",
        Type = PlaylistType.Music,
    };

    public static readonly PlaylistEntity MusicPlaylistForMultimediaDelete = new()
    {
        Id = Guid.Parse(input: "df7fa7e8-df17-4f61-b93d-189eae0cbdc6"),
        Title = "Rock Playlist for Delete",
        Description = "A collection of rock songs",
        Type = PlaylistType.Music,
        PlaylistMultimedia = new List<PlaylistMultimediaEntity>()
    };

    public static readonly PlaylistEntity MusicPlaylistDelete = new()
    {
        Id = Guid.Parse(input: "ff7fa7e8-df17-4f61-b93d-189eae0cbdb7"),
        Title = "Rock Playlist for Delete",
        Description = "A collection of rock songs",
        Type = PlaylistType.Music,
        PlaylistMultimedia = new List<PlaylistMultimediaEntity>()
    };

    public static DbContext SeedPlaylist(this DbContext dbx)
    {
        dbx.Set<PlaylistEntity>()
           .AddRange(AudioBookPlaylist,
                     VideoPlaylist,
                     MusicPlaylist,
                     MusicPlaylistForMultimediaUpdate,
                     MusicPlaylistUpdate,
                     MusicPlaylistForMultimediaDelete,
                     MusicPlaylistDelete
                    );
        return dbx;
    }
}
