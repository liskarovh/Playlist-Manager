

using PlaylistManager.Common.Tests;
using PlaylistManager.Common.Tests.Seeds;
using PlaylistManager.DAL.Entities;
using PlaylistManager.DAL.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Xunit;
using Xunit.Abstractions;

namespace PlaylistManager.DAL.Tests;

/// <summary>
/// Base class for database-related tests
/// Provides a structured setup and teardown process for tests involving the `PlaylistManagerDbContext`.
/// </summary>

public class DbContextTestsBase : IAsyncLifetime
{
    /// <summary>
    /// Initializes the test database and redirects console output for logging.
    /// </summary>
    protected DbContextTestsBase(ITestOutputHelper output)
    {
        XUnitTestOutputConverter converter = new(output);
        Console.SetOut(converter);

        DbContextFactory = new DbContextSqLiteFactory(GetType().FullName!);
        PlaylistManagerDbContextSUT = DbContextFactory.CreateDbContext();
    }

    /// <summary>
    /// A factory that creates database context instances.
    /// Helps generate fresh `DbContext` instances for tests.
    /// </summary>
    protected IDbContextFactory<PlaylistManagerDbContext> DbContextFactory { get; }

    /// <summary>
    /// The `DbContext` instance used in tests.
    /// Represents the actual test database.
    /// </summary>
    protected PlaylistManagerDbContext PlaylistManagerDbContextSUT { get; }

    /// <summary>
    /// Prepares the database by resetting and seeding it before tests run.
    /// </summary>
    public async Task InitializeAsync()
    {
        await PlaylistManagerDbContextSUT.Database.EnsureDeletedAsync();
        await PlaylistManagerDbContextSUT.Database.EnsureCreatedAsync();

        await using var dbx = await DbContextFactory.CreateDbContextAsync();

        dbx.SeedAudioBook()
           .SeedMusic()
           .SeedVideoMedia();
        //.SeedPlaylist()
        //.SeedPlaylistMultimedia();
        await dbx.SaveChangesAsync();
    }

    /// <summary>
    /// Ensures the database is deleted and resources are released after tests run.
    /// </summary>
    public async Task DisposeAsync()
    {
        await PlaylistManagerDbContextSUT.Database.EnsureDeletedAsync();
        await PlaylistManagerDbContextSUT.DisposeAsync();
    }
}
