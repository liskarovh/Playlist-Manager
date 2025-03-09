using PlaylistManager.Common.Enums;
namespace PlaylistManager.DAL.Tests;
using PlaylistManager.DAL.Entities;
using PlaylistManager.Common.Tests.Seeds;
using Xunit;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Test class verifies CRUD operations on the `AudioBookEntity` within the `PlaylistManagerDbContext`.
/// Ensures that AudioBooks can be created, retrieved, updated, and deleted correctly.
/// Verifies constraints such as preventing the deletion of AudioBook used in Playlist.
/// </summary>

public class DbContextAudiobookTests(ITestOutputHelper output) : DbContextTestsBase(output)
{

    /// <summary>
    /// Tests if a new AudioBook entity is successfully added and persisted in the database.
    /// </summary>
    [Fact]
    public async Task AddNew_AudioBook_Persisted()
    {
        //Arrange
        AudioBookEntity entity = new()
        {
            // Made up AudioBook, Guid is random
            Id = Guid.Parse("ddf0fc90-4e4e-47a3-ad1c-aa931e1b4b77"),
            Title = "Audiobook Title",
            Description = "A detailed and useful description of the AudioBook.",
            Duration = 314,
            Author = "Author Name",
            ReleaseYear = 1989,
            Url = "https://isbnsearch.org/isbn/9781835881224",
            Genre = AudioBookGenre.NonFiction,
            Format = AudioFormat.Aac

        };

        //Act
        PlaylistManagerDbContextSUT.AudioBooks.Add(entity);
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actualEntities = await dbx.AudioBooks.SingleAsync(i => i.Id == entity.Id);
        Assert.Equal(entity, actualEntities);
    }

    /// <summary>
    /// Tests if the seeded Audiobook "Dune" exists in the database.
    /// </summary>
    [Fact]
    public async Task GetAll_AudioBooks_ContainsSeededDune()
    {
        //Act
        var entities = await PlaylistManagerDbContextSUT.AudioBooks.ToArrayAsync();

        //Assert
        Assert.Contains(AudioBookSeeds.Dune, entities);
    }


    /// <summary>
    /// Tests if retrieving an AudioBook by its ID correctly returns the expected entity.
    /// </summary>
    [Fact]
    public async Task GetById_AudioBook_DuneRetrieved()
    {
        //Act
        var entity = await PlaylistManagerDbContextSUT.AudioBooks.SingleAsync(i => i.Id == AudioBookSeeds.Dune.Id);

        //Assert
        Assert.Equal(AudioBookSeeds.Dune, entity);
    }

    /// <summary>
    /// Tests if an updated AudioBook entity is correctly persisted in the database.
    /// </summary>
    [Fact]
    public async Task Update_AudioBook_Persisted()
    {
        //Arrange
        var baseEntity = AudioBookSeeds.ReadyPlayerOneUpdate;
        var entity = baseEntity with
        {
            Title = baseEntity + "Updated",
            Description = baseEntity + "Updated",

        };

        //Act
        PlaylistManagerDbContextSUT.AudioBooks.Update(entity);
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actualEntity = await dbx.AudioBooks.SingleAsync(i => i.Id == entity.Id);
        Assert.Equal(entity, actualEntity);
    }

    /// <summary>
    /// Tests if an AudioBook entity can be successfully deleted from the database.
    /// </summary>

    [Fact]
    public async Task Delete_AudioBook_ReadyPlayerOneDeleted()
    {
        //Arrange
        var entityBase = AudioBookSeeds.ReadyPlayerOneDelete;

        //Act
        PlaylistManagerDbContextSUT.AudioBooks.Remove(entityBase);
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        //Assert
        Assert.False(await PlaylistManagerDbContextSUT.AudioBooks.AnyAsync(i => i.Id == entityBase.Id));
    }

    /// <summary>
    /// Tests if deleting an AudioBook by its ID removes it from the database.
    /// </summary>

    [Fact]
    public async Task DeleteById_AudioBook_ReadyPlayerOneDeleted()
    {
        //Arrange
        var entityBase = AudioBookSeeds.ReadyPlayerOneDelete;

        //Act
        PlaylistManagerDbContextSUT.AudioBooks.Remove(PlaylistManagerDbContextSUT.AudioBooks.Single(i => i.Id == entityBase.Id));
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        //Assert
        Assert.False(await PlaylistManagerDbContextSUT.AudioBooks.AnyAsync(i => i.Id == entityBase.Id));
    }

    /// <summary>
    /// Tests if attempting to delete an AudioBook that is used in a Playlist throws a `DbUpdateException`.
    /// </summary>

    [Fact]
    public async Task Delete_AudioBook_UsedInPlaylist_ThrowsDbUpdateException()
    {
        //Arrange
        var entityBase = AudioBookSeeds.Dune;

        //Act
        PlaylistManagerDbContextSUT.AudioBooks.Remove(entityBase);

        //Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () => await PlaylistManagerDbContextSUT.SaveChangesAsync());
    }
}

