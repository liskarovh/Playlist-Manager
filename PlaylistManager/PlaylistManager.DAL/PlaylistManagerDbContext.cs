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

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types
    /// exposed in DbSet properties on your derived context. The resulting model may be cached
    /// and re-used for subsequent instances of your derived context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PlaylistMultimediaEntity>()
                    .HasOne(pm => pm.Playlist)
                    .WithMany(p => p.PlaylistMultimedia)
                    .HasForeignKey(pm => pm.PlaylistId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();

        modelBuilder.Entity<PlaylistMultimediaEntity>()
                    .HasOne(pm => pm.Multimedia)
                    .WithMany()
                    .HasForeignKey(pm => pm.MultimediaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .IsRequired();
    }
}
