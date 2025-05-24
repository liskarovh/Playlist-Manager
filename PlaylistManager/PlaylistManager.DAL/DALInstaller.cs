using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlaylistManager.DAL.Factories;
using PlaylistManager.DAL.Mappers;
using PlaylistManager.DAL.Migrator;
using PlaylistManager.DAL.Options;
using PlaylistManager.DAL.Seeds;

namespace PlaylistManager.DAL;

public static class DALInstaller
{
    public static IServiceCollection AddDALServices(this IServiceCollection services, DALOptions options)
    {
        services.AddSingleton(options);

        if (options is null)
        {
            throw new InvalidOperationException("No persistence provider configured.");
        }

        if (string.IsNullOrEmpty(options.DatabaseDirectory))
        {
            throw new InvalidOperationException($"{nameof(options.DatabaseDirectory)} is not set.");
        }

        services.AddSingleton<IDbContextFactory<PlaylistManagerDbContext>>
            (_ => new DbContextSqLiteFactory(Path.Combine(options.DatabaseDirectory, "playlist-manager.db")));

        services.AddSingleton<IDbMigrator, DbMigrator>();
        services.AddSingleton<IDbSeeder, DbSeeder>();

        services.AddSingleton<AudioBookEntityMapper>();
        services.AddSingleton<MusicEntityMapper>();
        services.AddSingleton<PlaylistEntityMapper>();
        services.AddSingleton<PlaylistMultimediaEntityMapper>();
        services.AddSingleton<VideoMediaEntityMapper>();

        return services;
    }
}
