using Microsoft.EntityFrameworkCore;
using PlaylistManager.DAL.Entities;
using PlaylistManager.Common.Enums;

namespace PlaylistManager.Common.Tests.Seeds;

public static class PlaylistSeeds
{
    /// <summary>
    /// Represents an empty Playlist entity.
    /// </summary>
    public static readonly PlaylistEntity EmptyPlaylist = new()
    {
        Id = default,
        Title = default!,
        Description = default,
        Type = default!,
        PlaylistMultimedia = new List<PlaylistMultimediaEntity>()
    };

    /// <summary>
    /// Represents a Playlist entity for rock music.
    /// </summary>
    public static readonly PlaylistEntity MusicPlaylist = new()
    {
        Id = Guid.Parse("dfcdc163-a6fb-4edb-80b9-1de8f1461a85"),
        Title = "Rock Playlist",
        Description = "A collection of rock songs",
        Type = PlaylistType.Music
    };

    /// <summary>
    /// Represents a Playlist entity for favorite movies.
    /// </summary>
    public static readonly PlaylistEntity VideoPlaylist = new()
    {
        Id = Guid.Parse("78f505ba-ba72-4d84-8e8d-a08e29182192"),
        Title = "Favorite Movies",
        Description = "List of favorite sci-fi movies",
        Type = PlaylistType.Video
    };

    /// <summary>
    /// Represents a Playlist entity for fantasy audiobooks.
    /// </summary>
    public static readonly PlaylistEntity AudioBookPlaylist = new()
    {
        Id = Guid.Parse("341f5161-841e-498d-974c-a2cea40a3c79"),
        Title = "Fantasy Audiobooks",
        Description = "Audiobook journey through fantasy worlds",
        Type = PlaylistType.AudioBook
    };

    /// <summary>
    /// Represents an update of the MusicPlaylist entity.
    /// </summary>
    public static readonly PlaylistEntity MusicPlaylistForMultimediaUpdate = new()
    {
        Id = Guid.Parse("41b93013-7605-4ee1-bbfb-e1503f125825"),
        Title = "Rock Playlist Updated",
        Description = "Updated best of rock 2025",
        Type = PlaylistType.Music,
    };

    /// <summary>
    /// Represents an update of the MusicPlaylist entity.
    /// </summary>
    public static readonly PlaylistEntity MusicPlaylistUpdate = new()
    {
        Id = Guid.Parse("42b93013-7605-4ee1-bbfb-e1503f125826"),
        Title = "Rock Playlist Updated",
        Description = "Updated best of rock 2025",
        Type = PlaylistType.Music,
    };

    /// <summary>
    /// Represents a MusicPlaylist entity for deletion.
    /// </summary>
    public static readonly PlaylistEntity MusicPlaylistForMultimediaDelete = new()
    {
        Id = Guid.Parse("df7fa7e8-df17-4f61-b93d-189eae0cbdc6"),
        Title = "Rock Playlist for Delete",
        Description = "A collection of rock songs",
        Type = PlaylistType.Music,
        PlaylistMultimedia = new List<PlaylistMultimediaEntity>()
    };

    /// <summary>
    /// Represents a MusicPlaylist entity for deletion.
    /// </summary>
    public static readonly PlaylistEntity MusicPlaylistDelete = new()
    {
        Id = Guid.Parse("ff7fa7e8-df17-4f61-b93d-189eae0cbdb7"),
        Title = "Rock Playlist for Delete",
        Description = "A collection of rock songs",
        Type = PlaylistType.Music,
        PlaylistMultimedia = new List<PlaylistMultimediaEntity>()
    };

    /// <summary>
    /// Seeds the database context with Playlist entities.
    /// </summary>
    /// <param name="dbx">The database context to seed.</param>
    /// <returns>The seeded database context.</returns>
    public static DbContext SeedPlaylist(this DbContext dbx)
    {
        dbx.Set<PlaylistEntity>()
           .AddRange(
                     AudioBookPlaylist,
                     VideoPlaylist,
                     MusicPlaylist,
                     MusicPlaylistForMultimediaUpdate,
                     MusicPlaylistUpdate,
                     MusicPlaylistForMultimediaDelete,
                     MusicPlaylistDelete
                     // EmptyPlaylist
                    );
        return dbx;
    }
}
