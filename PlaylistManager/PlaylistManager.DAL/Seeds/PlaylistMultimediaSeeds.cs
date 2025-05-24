using Microsoft.EntityFrameworkCore;
using PlaylistManager.DAL.Entities;

namespace PlaylistManager.DAL.Seeds;

public static class PlaylistMultimediaSeeds
{
    public static readonly PlaylistMultimediaEntity MusicPlaylist_BohemianRhapsody = new()
    {
        Id = Guid.Parse(input: "40b4f1e8-5605-4c3e-8f4c-c3cb105094a7"),
        PlaylistId = PlaylistSeeds.MusicPlaylist.Id,
        Playlist = PlaylistSeeds.MusicPlaylist,
        MultimediaId = MusicSeeds.BohemianRhapsody.Id,
        Multimedia = MusicSeeds.BohemianRhapsody
    };

    public static readonly PlaylistMultimediaEntity MusicPlaylist_AmericanIdiot = new()
    {
        Id = Guid.Parse(input: "77e70961-178e-4ce6-b6a3-94bb9f7475ea"),
        PlaylistId = PlaylistSeeds.MusicPlaylist.Id,
        Playlist = PlaylistSeeds.MusicPlaylist,
        MultimediaId = MusicSeeds.AmericanIdiot.Id,
        Multimedia = MusicSeeds.AmericanIdiot
    };

    public static readonly PlaylistMultimediaEntity VideoPlaylist_TheMatrix = new()
    {
        Id = Guid.Parse(input: "a77defd1-8bc4-455e-bc4f-4a3add1ae820"),
        PlaylistId = PlaylistSeeds.VideoPlaylist.Id,
        Playlist = PlaylistSeeds.VideoPlaylist,
        MultimediaId = VideoMediaSeeds.Matrix.Id,
        Multimedia = VideoMediaSeeds.Matrix
    };

    public static readonly PlaylistMultimediaEntity AudioBookPlaylist_Dune = new()
    {
        Id = Guid.Parse(input: "8b09152d-a5ad-4cc6-a5da-7f8f3e1784cc"),
        PlaylistId = PlaylistSeeds.AudioBookPlaylist.Id,
        Playlist = PlaylistSeeds.AudioBookPlaylist,
        MultimediaId = AudioBookSeeds.Dune.Id,
        Multimedia = AudioBookSeeds.Dune
    };

    static PlaylistMultimediaSeeds()
    {
        PlaylistSeeds.MusicPlaylist
                     .PlaylistMultimedia.Add(MusicPlaylist_BohemianRhapsody);

        PlaylistSeeds.MusicPlaylist
                     .PlaylistMultimedia.Add(MusicPlaylist_AmericanIdiot);

        PlaylistSeeds.VideoPlaylist
                     .PlaylistMultimedia.Add(VideoPlaylist_TheMatrix);

        PlaylistSeeds.AudioBookPlaylist
                     .PlaylistMultimedia.Add(AudioBookPlaylist_Dune);
    }

    public static DbContext SeedPlaylistMultimedia(this DbContext dbContext)
    {
        dbContext.Set<PlaylistMultimediaEntity>()
                 .AddRange(MusicPlaylist_BohemianRhapsody,
                           MusicPlaylist_AmericanIdiot,
                           VideoPlaylist_TheMatrix,
                           AudioBookPlaylist_Dune);
        return dbContext;
    }
}
