using Microsoft.EntityFrameworkCore.Design;

namespace PlaylistManager.DAL.Factories;

/// <summary>
/// Factory class for creating instances of <see cref="PlaylistManagerDbContext"/> at design time.
/// </summary>
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PlaylistManagerDbContext>
{
    private readonly DbContextSqLiteFactory _dbContextSqLiteFactory = new("playlistmanager.db");

    /// <summary>
    /// Creates a new instance of <see cref="PlaylistManagerDbContext"/> with the configured options.
    /// </summary>
    /// <param name="args">Arguments for creating the DbContext.</param>
    /// <returns>A new instance of <see cref="PlaylistManagerDbContext"/>.</returns>
    public PlaylistManagerDbContext CreateDbContext(string[] args) => _dbContextSqLiteFactory.CreateDbContext();
}
