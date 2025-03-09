
using PlaylistManager.Common.Enums;
namespace PlaylistManager.DAL.Tests;
using PlaylistManager.DAL.Entities;
using PlaylistManager.Common.Tests.Seeds;
using Xunit;
using Xunit.Abstractions;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Test class verifies CRUD operations on the `VideoMediaEntity` within the `PlaylistManagerDbContext`.
/// Ensures that VideoMedia can be created, retrieved, updated, and deleted correctly.
/// Verifies constraints such as preventing the deletion of VideoMedia used in Playlist.
/// </summary>
///

public class DbContextVideoMediaTests(ITestOutputHelper output) : DbContextTestsBase(output)
{

    /// <summary>
    /// Tests if a new VideoMedia entity is successfully added and persisted in the database.
    /// </summary>
    [Fact]
    public async Task AddNew_VideoMedia_Persisted()
    {
        //Arrange
        VideoMediaEntity entity = new()
        {
            //VideoMedia details, Guid is random, made up
            Id = Guid.Parse("80fcf7ad-f2d5-481a-a830-d5fefb5db616"),
            Title = "A very nice movie",
            Description = "A very good movie about a very nice guy.",
            Duration = 7302,
            Author = "Author Name",
            ReleaseYear = 2000,
            Url = "https://en.wikipedia.org/wiki/Catwoman_(film)",
            Genre = VideoGenre.Other,
            Format = VideoFormat.Mkv
        };

        //Act
        PlaylistManagerDbContextSUT.VideoMedia.Add(entity);
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actualEntities = await dbx.VideoMedia.SingleAsync(i => i.Id == entity.Id);
        Assert.Equal(entity, actualEntities);
    }

    /// <summary>
    /// Tests if the seeded VideoMedia "Shining" exists in the database.
    /// </summary>
    [Fact]
    public async Task GetAll_VideoMedia_ContainsSeededShining()
    {
        //Act
        var entities = await PlaylistManagerDbContextSUT.VideoMedia.ToArrayAsync();

        //Assert
        Assert.Contains(VideoMediaSeeds.Shining, entities);
    }


    /// <summary>
    /// Tests if retrieving a VideoMedia by its ID correctly returns the expected entity.
    /// </summary>
    [Fact]
    public async Task GetById_VideoMedia_ShiningRetrieved()
    {
        //Act
        var entity = await PlaylistManagerDbContextSUT.VideoMedia.SingleAsync(i => i.Id == VideoMediaSeeds.Shining.Id);

        //Assert
        Assert.Equal(VideoMediaSeeds.Shining, entity);
    }

    /// <summary>
    /// Tests if an updated VideoMedia entity is correctly persisted in the database.
    /// </summary>
    [Fact]
    public async Task Update_VideoMedia_Persisted()
    {
        //Arrange
        var baseEntity = VideoMediaSeeds.MatrixUpdate;
        var entity = baseEntity with
        {
            Title = baseEntity + "Updated",
            Description = baseEntity + "Updated",

        };

        //Act
        PlaylistManagerDbContextSUT.VideoMedia.Update(entity);
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        //Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actualEntity = await dbx.VideoMedia.SingleAsync(i => i.Id == entity.Id);
        Assert.Equal(entity, actualEntity);
    }

    /// <summary>
    /// Tests if a VideoMedia entity can be successfully deleted from the database.
    /// </summary>

    [Fact]
    public async Task Delete_VideoMedia_MatrixDeleted()
    {
        //Arrange
        var entityBase = VideoMediaSeeds.MatrixDelete;

        //Act
        PlaylistManagerDbContextSUT.VideoMedia.Remove(entityBase);
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        //Assert
        Assert.False(await PlaylistManagerDbContextSUT.VideoMedia.AnyAsync(i => i.Id == entityBase.Id));
    }

    /// <summary>
    /// Tests if deleting a VideoMedia by its ID removes it from the database.
    /// </summary>

    [Fact]
    public async Task DeleteById_VideoMedia_MatrixDeleted()
    {
        //Arrange
        var entityBase = VideoMediaSeeds.MatrixDelete;

        //Act
        PlaylistManagerDbContextSUT.VideoMedia.Remove(PlaylistManagerDbContextSUT.VideoMedia.Single(i => i.Id == entityBase.Id));
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        //Assert
        Assert.False(await PlaylistManagerDbContextSUT.VideoMedia.AnyAsync(i => i.Id == entityBase.Id));
    }

    /// <summary>
    /// Tests if attempting to delete a VideoMedia that is used in a Playlist throws a `DbUpdateException`.
    /// </summary>

    [Fact]
    public async Task Delete_VideoMedia_UsedInPlaylist_ThrowsDbUpdateException()
    {
        //Arrange
        var entityBase = VideoMediaSeeds.Shining;

        //Act
        PlaylistManagerDbContextSUT.VideoMedia.Remove(entityBase);

        //Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () => await PlaylistManagerDbContextSUT.SaveChangesAsync());
    }

}

