
using PlaylistManager.Common.Enums;
namespace PlaylistManager.DAL.Tests;
using PlaylistManager.DAL.Entities;
using PlaylistManager.Common.Tests.Seeds;
using Xunit;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Test class verifies CRUD operations on the `MusicEntity` within the `PlaylistManagerDbContext`.
/// Ensures that Music can be created, retrieved, updated, and deleted correctly.
/// Verifies constraints such as preventing the deletion of Music used in Playlist.
/// </summary>

public class DbContextMusicTests(ITestOutputHelper output) : DbContextTestsBase(output)
{

    /// <summary>
    /// Tests if a new Music entity is successfully added and persisted in the database.
    /// </summary>
    [Fact]
    public async Task AddNew_Music_Persisted()
    {
        //Arrange
        MusicEntity entity = new()
        {
            //Music details, Guid is random
            Id = Guid.Parse(input: "0e99c068-7a0d-45d5-b084-5652d962bacb"),
            Title = "What A Wonderful World",
            Description = "Flowy calming jazz song.",
            Duration = 150,
            Author = "Louis Armstrong",
            ReleaseYear = 1967,
            Url = "https://www.youtube.com/watch?v=VqhCQZaH4Vs",
            Genre = MusicGenre.Jazz,
            Format = AudioFormat.Mp3
        };

        //Act
        PlaylistManagerDbContextSUT.Music.Add(entity);
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actualEntities = await dbx.Music.SingleAsync(i => i.Id == entity.Id);
        Assert.Equal(entity, actualEntities);
    }

    /// <summary>
    /// Tests if the seeded Music "Vltava" exists in the database.
    /// </summary>
    [Fact]
    public async Task GetAll_Music_ContainsSeededVltava()
    {
        //Act
        var entities = await PlaylistManagerDbContextSUT.Music.ToArrayAsync();

        //Assert
        Assert.Contains(MusicSeeds.Vltava, entities);
    }


    /// <summary>
    /// Tests if retrieving a Music by its ID correctly returns the expected entity.
    /// </summary>
    [Fact]
    public async Task GetById_Music_VltavaRetrieved()
    {
        //Act
        var entity = await PlaylistManagerDbContextSUT.Music.SingleAsync(i => i.Id == MusicSeeds.Vltava.Id);

        //Assert
        Assert.Equal(MusicSeeds.Vltava, entity);
    }

    /// <summary>
    /// Tests if an updated Music entity is correctly persisted in the database.
    /// </summary>
    [Fact]
    public async Task Update_Music_Persisted()
    {
        //Arrange
        var baseEntity = MusicSeeds.BohemianRhapsodyUpdate;
        var entity = baseEntity with
        {
            Title = baseEntity + "Updated",
            Description = baseEntity + "Updated",

        };

        //Act
        PlaylistManagerDbContextSUT.Music.Update(entity);
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actualEntity = await dbx.Music.SingleAsync(i => i.Id == entity.Id);
        Assert.Equal(entity, actualEntity);
    }

    /// <summary>
    /// Tests if a Music entity can be successfully deleted from the database.
    /// </summary>

    [Fact]
    public async Task Delete_Music_BohemianRhapsodyDeleted()
    {
        //Arrange
        var entityBase = MusicSeeds.BohemianRhapsodyDelete;

        //Act
        PlaylistManagerDbContextSUT.Music.Remove(entityBase);
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        //Assert
        Assert.False(await PlaylistManagerDbContextSUT.Music.AnyAsync(i => i.Id == entityBase.Id));
    }

    /// <summary>
    /// Tests if deleting Music by its ID removes it from the database.
    /// </summary>

    [Fact]
    public async Task DeleteById_Music_BohemianRhapsodyDeleted()
    {
        //Arrange
        var entityBase = MusicSeeds.BohemianRhapsodyDelete;

        //Act
        PlaylistManagerDbContextSUT.Music.Remove(PlaylistManagerDbContextSUT.Music.Single(i => i.Id == entityBase.Id));
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        //Assert
        Assert.False(await PlaylistManagerDbContextSUT.Music.AnyAsync(i => i.Id == entityBase.Id));
    }

    /// <summary>
    /// Tests if attempting to delete Music that is used in a Playlist throws a `DbUpdateException`.
    /// </summary>

    [Fact]
    public async Task Delete_Music_UsedInPlaylist_ThrowsDbUpdateException()
    {
        //Arrange
        var entityBase = MusicSeeds.Vltava;

        //Act
        PlaylistManagerDbContextSUT.Music.Remove(entityBase);

        //Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () => await PlaylistManagerDbContextSUT.SaveChangesAsync());
    }

}
