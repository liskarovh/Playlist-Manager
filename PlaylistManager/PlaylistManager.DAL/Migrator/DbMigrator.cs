// Copyright (c).NET Foundation and contributors.All rights reserved.
// Licensed under the MIT license.See LICENSE file in the project root for full license information.



using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PlaylistManager.DAL.Options;
namespace PlaylistManager.DAL.Migrator;

public class DbMigrator(IDbContextFactory<PlaylistManagerDbContext> dbContextFactory, IOptions<DALOptions> options)
    : IDbMigrator
{
    public void Migrate()
    {
        using PlaylistManagerDbContext dbContext = dbContextFactory.CreateDbContext();

        if(options.Value.RecreateDatabaseEachTime)
        {
            dbContext.Database.EnsureDeleted();
        }

        // Ensures that database is created applying the latest state
        // Application of migration later on may fail

        // Ensures the database is created based on the current model.
        // This method bypasses the EF Core migrations system and creates the schema directly.
        // Suitable for development, testing, or simple apps where migrations are not needed
        // In such cases, use dbContext.Database.Migrate() instead.
        dbContext.Database.EnsureCreated();
    }
}


