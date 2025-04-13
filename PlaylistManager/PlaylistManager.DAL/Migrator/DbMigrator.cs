using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PlaylistManager.DAL.Options;

namespace PlaylistManager.DAL.Migrator;

public class DbMigrator(IDbContextFactory<PlaylistManagerDbContext> dbContextFactory, IOptions<DALOptions> options) : IDbMigrator
{
    public void Migrate()
    {
        using PlaylistManagerDbContext dbContext = dbContextFactory.CreateDbContext();

        if (options.Value.RecreateDatabaseEachTime)
        {
            dbContext.Database.EnsureDeleted();
        }

        // This method bypasses the EF Core migrations system and creates the schema directly.
        // It ensures the database is created based on the current model.
        dbContext.Database.EnsureCreated();
    }
}
