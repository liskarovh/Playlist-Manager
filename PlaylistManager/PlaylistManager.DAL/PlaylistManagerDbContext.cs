using Microsoft.EntityFrameworkCore;
using PlaylistManager.DAL.Entities;

namespace PlaylistManager.DAL;

public class PlaylistManagerDbContext(DbContextOptions contextOptions) : DbContext(contextOptions)
{
    public DbSet<PlaylistMultimediaEntity> PlaylistMultimedia => Set<PlaylistMultimediaEntity>();

    public DbSet<PlaylistEntity> Playlists => Set<PlaylistEntity>();

    public DbSet<MultimediaBaseEntity> MultimediaBaseEntities => Set<MultimediaBaseEntity>();

    public DbSet<AudioMediaEntity> AudioMedia => Set<AudioMediaEntity>();

    public DbSet<AudioBookEntity> AudioBooks => Set<AudioBookEntity>();

    public DbSet<VideoMediaEntity> VideoMedia => Set<VideoMediaEntity>();

    public DbSet<MusicEntity> Music => Set<MusicEntity>();

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
