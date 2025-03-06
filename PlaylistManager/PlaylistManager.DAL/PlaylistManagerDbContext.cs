using Microsoft.EntityFrameworkCore;
using PlaylistManager.Common.Enums;
using PlaylistManager.DAL.Entities;

namespace PlaylistManager.DAL;

/// <summary>
/// Represents the database context for the Playlist Manager application.
/// </summary>
public class PlaylistManagerDbContext(DbContextOptions contextOptions) : DbContext(contextOptions)
{
    /// <summary>
    /// Gets or sets the DbSet for PlaylistMultimediaEntity.
    /// </summary>
    public DbSet<PlaylistMultimediaEntity> PlaylistMultimedia => Set<PlaylistMultimediaEntity>();

    /// <summary>
    /// Gets or sets the DbSet for PlaylistBaseEntity.
    /// </summary>
    public DbSet<PlaylistBaseEntity> Playlists => Set<PlaylistBaseEntity>();

    /// <summary>
    /// Gets or sets the DbSet for MusicPlaylistEntity.
    /// </summary>
    public DbSet<MusicPlaylistEntity> MusicPlaylists => Set<MusicPlaylistEntity>();

    /// <summary>
    /// Gets or sets the DbSet for VideoPlaylistEntity.
    /// </summary>
    public DbSet<VideoPlaylistEntity> VideoPlaylists => Set<VideoPlaylistEntity>();

    /// <summary>
    /// Gets or sets the DbSet for AudioBookPlaylistEntity.
    /// </summary>
    public DbSet<AudioBookPlaylistEntity> AudioBookPlaylists => Set<AudioBookPlaylistEntity>();

    /// <summary>
    /// Gets or sets the DbSet for MultimediaBaseEntity.
    /// </summary>
    public DbSet<MultimediaBaseEntity> MultimediaBaseEntities => Set<MultimediaBaseEntity>();

    /// <summary>
    /// Gets or sets the DbSet for AudioMediaEntity.
    /// </summary>
    public DbSet<AudioMediaEntity> AudioMedia => Set<AudioMediaEntity>();

    /// <summary>
    /// Gets or sets the DbSet for AudioBookEntity.
    /// </summary>
    public DbSet<AudioBookEntity> AudioBooks => Set<AudioBookEntity>();

    /// <summary>
    /// Gets or sets the DbSet for VideoMediaEntity.
    /// </summary>
    public DbSet<VideoMediaEntity> VideoMedia => Set<VideoMediaEntity>();

    /// <summary>
    /// Gets or sets the DbSet for MusicEntity.
    /// </summary>
    public DbSet<MusicEntity> Music => Set<MusicEntity>();
}
