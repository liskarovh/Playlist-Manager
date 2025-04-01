using Microsoft.EntityFrameworkCore;

namespace PlaylistManager.DAL.Factories;

public class DbContextSqLiteFactory : IDbContextFactory<PlaylistManagerDbContext>
{
    private readonly DbContextOptionsBuilder<PlaylistManagerDbContext> _contextOptionsBuilder = new();

    public PlaylistManagerDbContext CreateDbContext() => new(_contextOptionsBuilder.Options);

    public DbContextSqLiteFactory(string databaseName) => _contextOptionsBuilder.UseSqlite($"Data Source={databaseName};Cache=Shared");
}
