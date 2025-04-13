using PlaylistManager.DAL;
using PlaylistManager.DAL.Factories;
using PlaylistManager.DAL.UnitOfWork;
using PlaylistManager.BL.Mappers;
using PlaylistManager.Common.Tests;
using PlaylistManager.Common.Tests.Seeds;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace PlaylistManager.BL.Tests;

public class FacadeTestsBase : IAsyncLifetime
{
    protected FacadeTestsBase(ITestOutputHelper output)
    {
        XUnitTestOutputConverter converter = new(output);
        Console.SetOut(converter);

        DbContextFactory = new DbContextSqLiteFactory($"{GetType().FullName}.db");

        MediumModelMapper = new MediumModelMapper();
        PlaylistModelMapper = new PlaylistModelMapper();

        UnitOfWorkFactory = new UnitOfWorkFactory(DbContextFactory);
    }

    protected IDbContextFactory<PlaylistManagerDbContext> DbContextFactory { get; }

    protected MediumModelMapper MediumModelMapper { get; }
    protected PlaylistModelMapper PlaylistModelMapper { get; }
    protected UnitOfWorkFactory UnitOfWorkFactory { get; }

    public async Task InitializeAsync()
    {
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        await dbx.Database.EnsureDeletedAsync();
        await dbx.Database.EnsureCreatedAsync();

        dbx.SeedMusic()
           .SeedPlaylist()
           .SeedPlaylistMultimedia();
        await dbx.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        await dbx.Database.EnsureDeletedAsync();
    }
}
