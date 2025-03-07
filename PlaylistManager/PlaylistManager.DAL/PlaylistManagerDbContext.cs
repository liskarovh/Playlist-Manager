using Microsoft.EntityFrameworkCore;
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
    /// Gets or sets the DbSet for PlaylistEntity.
    /// </summary>
    public DbSet<PlaylistEntity> Playlists => Set<PlaylistEntity>();

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
