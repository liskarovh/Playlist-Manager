using Microsoft.EntityFrameworkCore;

namespace PlaylistManager.DAL.Factories;

/// <summary>
/// Factory class for creating instances of <see cref="PlaylistManagerDbContext"/> using SQLite.
/// </summary>
public class DbContextSqLiteFactory : IDbContextFactory<PlaylistManagerDbContext>
{
    private readonly DbContextOptionsBuilder<PlaylistManagerDbContext> _contextOptionsBuilder = new();

    /// <summary>
    /// Creates a new instance of <see cref="PlaylistManagerDbContext"/> with the configured options.
    /// </summary>
    /// <returns>A new instance of <see cref="PlaylistManagerDbContext"/>.</returns>
    public PlaylistManagerDbContext CreateDbContext() => new(_contextOptionsBuilder.Options);

    /// <summary>
    /// Initializes a new instance of the <see cref="DbContextSqLiteFactory"/> class.
    /// </summary>
    /// <param name="databaseName">The name of the database.</param>
    public DbContextSqLiteFactory(string databaseName) => _contextOptionsBuilder.UseSqlite($"Data Source={databaseName};Cache=Shared");
}
