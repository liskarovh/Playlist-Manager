using Microsoft.EntityFrameworkCore;
using PlaylistManager.DAL.Entities;

namespace PlaylistManager.Common.Tests.Seeds;

/// <summary>
/// Provides seed data for PlaylistMultimedia entities.
/// </summary>
public static class PlaylistMultimediaSeeds
{
    /// <summary>
    /// Represents an empty PlaylistMultimedia entity.
    /// </summary>
    public static readonly PlaylistMultimediaEntity EmptyPlaylistMultimedia = new()
    {
        Id = default,
        PlaylistId = default,
        MultimediaId = default,
        Playlist = default!,
        Multimedia = default!
    };

    /// <summary>
    /// Represents the Bohemian Rhapsody multimedia in the Music playlist.
    /// </summary>
    public static readonly PlaylistMultimediaEntity MusicPlaylist_BohemianRhapsody = new()
    {
        Id = Guid.Parse("40b4f1e8-5605-4c3e-8f4c-c3cb105094a7"),
        PlaylistId = PlaylistSeeds.MusicPlaylist.Id,
        Playlist = PlaylistSeeds.MusicPlaylist,
        MultimediaId = MusicSeeds.BohemianRhapsody.Id,
        Multimedia = MusicSeeds.BohemianRhapsody
    };

    /// <summary>
    /// Represents the American Idiot multimedia in the Music playlist.
    /// </summary>
    public static readonly PlaylistMultimediaEntity MusicPlaylist_AmericanIdiot = new()
    {
        Id = Guid.Parse("77e70961-178e-4ce6-b6a3-94bb9f7475ea"),
        PlaylistId = PlaylistSeeds.MusicPlaylist.Id,
        Playlist = PlaylistSeeds.MusicPlaylist,
        MultimediaId = MusicSeeds.AmericanIdiot.Id,
        Multimedia = MusicSeeds.AmericanIdiot
    };

    /// <summary>
    /// Represents The Matrix multimedia in the Video playlist.
    /// </summary>
    public static readonly PlaylistMultimediaEntity VideoPlaylist_TheMatrix = new()
    {
        Id = Guid.Parse("a77defd1-8bc4-455e-bc4f-4a3add1ae820"),
        PlaylistId = PlaylistSeeds.VideoPlaylist.Id,
        Playlist = PlaylistSeeds.VideoPlaylist,
        MultimediaId = VideoMediaSeeds.Matrix.Id,
        Multimedia = VideoMediaSeeds.Matrix
    };

    /// <summary>
    /// Represents the Dune audiobook in the Audiobook playlist.
    /// </summary>
    public static readonly PlaylistMultimediaEntity AudioBookPlaylist_Dune = new()
    {
        Id = Guid.Parse("8b09152d-a5ad-4cc6-a5da-7f8f3e1784cc"),
        PlaylistId = PlaylistSeeds.AudioBookPlaylist.Id,
        Playlist = PlaylistSeeds.AudioBookPlaylist,
        MultimediaId = AudioBookSeeds.Dune.Id,
        Multimedia = AudioBookSeeds.Dune
    };

    /// <summary>
    /// Represents an updated Bohemian Rhapsody multimedia in the Music playlist.
    /// </summary>
    public static readonly PlaylistMultimediaEntity MusicPlaylist_BohemianRhapsodyUpdate =
        MusicPlaylist_BohemianRhapsody with
        {
            Id = Guid.Parse("87f6a5cf-519b-462d-9872-3d17f1c1b67b"),
            PlaylistId = PlaylistSeeds.MusicPlaylistUpdate.Id,
            Playlist = PlaylistSeeds.MusicPlaylistUpdate
        };

    /// <summary>
    /// Represents a deleted Bohemian Rhapsody multimedia in the Music playlist.
    /// </summary>
    public static readonly PlaylistMultimediaEntity MusicPlaylist_BohemianRhapsodyDelete =
        MusicPlaylist_BohemianRhapsody with
        {
            Id = Guid.Parse("d660ede9-513c-44e9-9390-71dca1aeadd7"),
            PlaylistId = PlaylistSeeds.MusicPlaylistDelete.Id,
            Playlist = PlaylistSeeds.MusicPlaylistDelete
        };

    /// <summary>
    /// Static constructor to initialize the PlaylistMultimediaSeeds class.
    /// Adds predefined PlaylistMultimedia entities to their respective playlists.
    /// </summary>
    static PlaylistMultimediaSeeds()
    {
        PlaylistSeeds.MusicPlaylist
                     .PlaylistMultimedia.Add(MusicPlaylist_BohemianRhapsody);

        PlaylistSeeds.MusicPlaylist
                     .PlaylistMultimedia.Add(MusicPlaylist_AmericanIdiot);

        PlaylistSeeds.MusicPlaylistUpdate
                     .PlaylistMultimedia.Add(MusicPlaylist_BohemianRhapsodyUpdate);

        PlaylistSeeds.MusicPlaylistDelete
                     .PlaylistMultimedia.Add(MusicPlaylist_BohemianRhapsodyDelete);

        PlaylistSeeds.VideoPlaylist
                     .PlaylistMultimedia.Add(VideoPlaylist_TheMatrix);

        PlaylistSeeds.AudioBookPlaylist
                     .PlaylistMultimedia.Add(AudioBookPlaylist_Dune);
    }

    /// <summary>
    /// Seeds the PlaylistMultimedia entities into the specified DbContext.
    /// </summary>
    /// <param name="dbContext">The DbContext to seed.</param>
    /// <returns>The seeded DbContext.</returns>
    public static DbContext SeedPlaylistMultimedia(this DbContext dbContext)
    {
        dbContext.Set<PlaylistMultimediaEntity>()
                 .AddRange(
                           MusicPlaylist_BohemianRhapsody,
                           MusicPlaylist_AmericanIdiot,
                           VideoPlaylist_TheMatrix,
                           AudioBookPlaylist_Dune,
                           MusicPlaylist_BohemianRhapsodyUpdate,
                           MusicPlaylist_BohemianRhapsodyDelete
        // EmptyPlaylistMultimedia,
                          );
        return dbContext;
    }
}
