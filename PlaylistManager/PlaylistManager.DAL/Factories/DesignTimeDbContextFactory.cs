using Microsoft.EntityFrameworkCore.Design;

namespace PlaylistManager.DAL.Factories;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PlaylistManagerDbContext>
{
    private readonly DbContextSqLiteFactory _dbContextSqLiteFactory = new("playlistmanager.db");

    public PlaylistManagerDbContext CreateDbContext(string[] args) => _dbContextSqLiteFactory.CreateDbContext();
}
