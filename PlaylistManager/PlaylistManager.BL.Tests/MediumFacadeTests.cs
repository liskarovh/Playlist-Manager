using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;
using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Facades;
using PlaylistManager.BL.Models;
using PlaylistManager.Common.Tests;
using PlaylistManager.Common.Tests.Seeds;

namespace PlaylistManager.BL.Tests;

public class MediumFacadeTests : FacadeTestsBase
{
    private readonly IMediumFacade _mediumFacadeSUT;

    public MediumFacadeTests(ITestOutputHelper output) : base(output)
    {
        _mediumFacadeSUT = new MediumFacade(UnitOfWorkFactory, MediumModelMapper);
    }

    [Fact]
    public async Task GetById_NonExistentMedium_ReturnsNull()
    {
        // Arrange
        var seededMediumInsidePlaylist = PlaylistMultimediaSeeds.EmptyPlaylistMultimedia;

        // Act
        var result = await _mediumFacadeSUT.GetAsync(seededMediumInsidePlaylist.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAll_Single_SeededMusicBohemianRhapsodyNameOnly()
    {
        // Arrange
        var seededMediumInsidePlaylist = PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody;

        // Act
        var media = await _mediumFacadeSUT.GetAsync();
        var medium = media.SingleOrDefault(m => m.Id == seededMediumInsidePlaylist.Id);

        // Assert
        DeepAssert.Equal(MediumModelMapper.MapToNameOnly(seededMediumInsidePlaylist), medium);
    }

    [Fact]
    public async Task GetById_SeededMusicBohemianRhapsodyDetailed()
    {
        // Arrange
        var seededMediumInsidePlaylist = PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody;

        // Act
        var result = await _mediumFacadeSUT.GetAsync(seededMediumInsidePlaylist.Id);

        // Assert
        DeepAssert.Equal(MediumModelMapper.MapToDetailModel(seededMediumInsidePlaylist), result);
    }

    [Fact]
    public async Task Delete_SeededMusicBohemianRhapsody_Deleted()
    {
        // Arrange
        var seededMediumInsidePlaylist = PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody;

        // Act
        await _mediumFacadeSUT.DeleteAsync(seededMediumInsidePlaylist.Id);

        // Assert
        await using var dbxAssert = await DbContextFactory.CreateDbContextAsync();
        Assert.False(await dbxAssert.Music.AnyAsync(i => i.Id == seededMediumInsidePlaylist.Id));
    }

    [Fact]
    public async Task Delete_SeededMusicBohemianRhapsody_NoThrow()
    {
        // Arrange
        var seededMediumInsidePlaylist = PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody;

        // Act & Assert
        var exception = await Record.ExceptionAsync(async () => await _mediumFacadeSUT.DeleteAsync(seededMediumInsidePlaylist.Id));
        Assert.Null(exception);
    }

    [Fact]
    public async Task TryToDeleteAlreadyDeleted_SeededMusicBohemianRhapsody_Throw()
    {
        // Arrange
        var seededMediumInsidePlaylist = PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody;
        await _mediumFacadeSUT.DeleteAsync(seededMediumInsidePlaylist.Id);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _mediumFacadeSUT.DeleteAsync(seededMediumInsidePlaylist.Id));
    }

    [Fact]
    public async Task NewMedium_InsertOrUpdate_MediumInserted()
    {
        // Arrange
        var medium = new MediumDetailedModel
        {
            Id = PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody.Id,
            MediumId = MusicSeeds.BohemianRhapsody.Id,
            PlaylistId = PlaylistSeeds.MusicPlaylist.Id,
            Title = MusicSeeds.BohemianRhapsody.Title,
            Author = MusicSeeds.BohemianRhapsody.Author,
            Description = MusicSeeds.BohemianRhapsody.Description,
            Url = MusicSeeds.BohemianRhapsody.Url,
            Duration = MusicSeeds.BohemianRhapsody.Duration,
            ReleaseYear = MusicSeeds.BohemianRhapsody.ReleaseYear,
            Format = MusicSeeds.BohemianRhapsody.Format.ToString(),
            Genre = MusicSeeds.BohemianRhapsody.Genre.ToString()
        };

        // Act
        medium = await _mediumFacadeSUT.SaveAsync(medium);

        // Assert
        await using var dbxAssert = await DbContextFactory.CreateDbContextAsync();
        var mediumFromDb = await dbxAssert.PlaylistMultimedia
                                          .Include(m => m.Multimedia)
                                          .SingleAsync(i => i.Id == medium.Id);
        var detailModel = MediumModelMapper.MapToDetailModel(mediumFromDb);
        DeepAssert.Equal(medium, detailModel);
    }

    [Fact]
    public async Task SeededMusicBohemianRhapsody_InsertOrUpdate_MediumUpdated()
    {
        // Arrange
        var medium = new MediumDetailedModel
        {
            Id = PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody.Id,
            MediumId = MusicSeeds.BohemianRhapsody.Id,
            Title = MusicSeeds.BohemianRhapsody.Title,
            PlaylistId = PlaylistSeeds.MusicPlaylist.Id,
            Author = MusicSeeds.BohemianRhapsody.Author,
            Description = MusicSeeds.BohemianRhapsody.Description,
            Url = MusicSeeds.BohemianRhapsody.Url,
            Duration = MusicSeeds.BohemianRhapsody.Duration,
            ReleaseYear = MusicSeeds.BohemianRhapsody.ReleaseYear,
            Format = MusicSeeds.BohemianRhapsody.Format.ToString(),
            Genre = MusicSeeds.BohemianRhapsody.Genre.ToString()
        };

        medium.Title += " Updated";
        medium.Author += " Updated";
        medium.Description += " Updated";

        // Act
        await _mediumFacadeSUT.SaveAsync(medium);

        // Assert
        await using var dbxAssert = await DbContextFactory.CreateDbContextAsync();
        var mediumFromDb = await dbxAssert.PlaylistMultimedia
                                          .Include(m => m.Multimedia)
                                          .SingleAsync(i => i.Id == medium.Id);
        DeepAssert.Equal(medium, MediumModelMapper.MapToDetailModel(mediumFromDb));
    }
}
