using Microsoft.EntityFrameworkCore;
using PlaylistManager.DAL.Entities;
using PlaylistManager.Common.Enums;

namespace PlaylistManager.DAL.Seeds;

public static class PlaylistSeeds
{
    public static readonly PlaylistEntity MusicPlaylist = new()
    {
        Id = Guid.Parse(input: "dfcdc163-a6fb-4edb-80b9-1de8f1461a85"),
        Title = "Rock Playlist",
        Description = "A collection of rock songs",
        Type = PlaylistType.Music,
        PlaylistMultimedia = new List<PlaylistMultimediaEntity>()
    };

    public static readonly PlaylistEntity VideoPlaylist = new()
    {
        Id = Guid.Parse(input: "78f505ba-ba72-4d84-8e8d-a08e29182192"),
        Title = "Favorite Movies",
        Description = "List of favorite sci-fi movies",
        Type = PlaylistType.Video,
        PlaylistMultimedia = new List<PlaylistMultimediaEntity>()
    };

    public static readonly PlaylistEntity AudioBookPlaylist = new()
    {
        Id = Guid.Parse(input: "341f5161-841e-498d-974c-a2cea40a3c79"),
        Title = "Fantasy Audiobooks",
        Description = "Audiobook journey through fantasy worlds",
        Type = PlaylistType.AudioBook,
        PlaylistMultimedia = new List<PlaylistMultimediaEntity>()
    };

    public static DbContext SeedPlaylist(this DbContext dbx)
    {
        dbx.Set<PlaylistEntity>()
           .AddRange(AudioBookPlaylist,
                     VideoPlaylist,
                     MusicPlaylist);
        return dbx;
    }
}
