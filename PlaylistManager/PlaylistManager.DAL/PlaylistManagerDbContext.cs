using Microsoft.EntityFrameworkCore;
using PlaylistManager.Common.Enums;
using PlaylistManager.DAL.Entities;

namespace PlaylistManager.DAL;

public class PlaylistManagerDbContext(DbContextOptions contextOptions) : DbContext(contextOptions)
{
    public DbSet<PlaylistEntity> Playlists => Set<PlaylistEntity>();
    public DbSet<PlaylistMultimediaEntity> PlaylistMultimedia => Set<PlaylistMultimediaEntity>();

    public DbSet<MultimediaBaseEntity> MultimediaBaseEntities => Set<MultimediaBaseEntity>();
    public DbSet<AudioMediaEntity> AudioMedia => Set<AudioMediaEntity>();
    public DbSet<AudioBookEntity> AudioBooks => Set<AudioBookEntity>();
    public DbSet<VideoMediaEntity> VideoMedia => Set<VideoMediaEntity>();
    public DbSet<MusicEntity> Music => Set<MusicEntity>();
}

