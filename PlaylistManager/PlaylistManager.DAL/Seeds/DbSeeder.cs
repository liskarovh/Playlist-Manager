using Microsoft.EntityFrameworkCore;
using PlaylistManager.DAL.Options;

namespace PlaylistManager.DAL.Seeds;

public class DbSeeder(IDbContextFactory<PlaylistManagerDbContext> dbContextFactory, DALOptions options)
    : IDbSeeder
{
    public void Seed() => SeedAsync(CancellationToken.None).GetAwaiter().GetResult();

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync(cancellationToken);

        if (options.SeedDemoData)
        {
            if (await dbContext.MultimediaBaseEntities.AnyAsync(cancellationToken))
            {
                return;
            }

            dbContext
                .SeedPlaylist()
                .SeedAudioBook()
                .SeedVideoMedia()
                .SeedMusic()
                .SeedPlaylistMultimedia();

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
