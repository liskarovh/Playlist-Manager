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

    [Fact]
    public async Task GetAsyncSummary_NoEntities_ReturnsEmptyCollection()
    {
        // Arrange
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        dbx.PlaylistMultimedia.RemoveRange(dbx.PlaylistMultimedia);
        await dbx.SaveChangesAsync();

        // Act
        var result = await _mediumFacadeSUT.GetAsyncSummary();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAsyncSummary_SeededMusicBohemianRhapsody_ReturnsCorrectSummary()
    {
        // Arrange
        var seededMediumInsidePlaylist = PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody;

        // Act
        var result = await _mediumFacadeSUT.GetAsyncSummary();

        // Assert
        Assert.NotNull(result);
        var summary = result.SingleOrDefault(m => m.Id == seededMediumInsidePlaylist.Id);
        Assert.NotNull(summary);
        DeepAssert.Equal(MediumModelMapper.MapToSummary(seededMediumInsidePlaylist), summary);
    }

    [Fact]
    public async Task GetAsyncSummary_MultipleSeededEntities_ReturnsAllSummaries()
    {
        // Arrange
        var expectedEntities = new[]
        {
            PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody, PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsodyDelete, PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsodyUpdate,
            PlaylistMultimediaSeeds.MusicPlaylist_AmericanIdiot, PlaylistMultimediaSeeds.AudioBookPlaylist_Dune, PlaylistMultimediaSeeds.VideoPlaylist_TheMatrix
        };

        // Act
        var result = await _mediumFacadeSUT.GetAsyncSummary();

        // Assert
        Assert.NotNull(result);
        IEnumerable<MediumSummaryModel> mediumSummaryModels = result.ToList();
        Assert.Equal(expectedEntities.Length, mediumSummaryModels.Count());

        foreach (var expectedEntity in expectedEntities)
        {
            var summary = mediumSummaryModels.SingleOrDefault(m => m.Id == expectedEntity.Id);
            Assert.NotNull(summary);
            DeepAssert.Equal(MediumModelMapper.MapToSummary(expectedEntity), summary);
        }
    }

    [Fact]
    public async Task GetMediaByPlaylistIdAsync_ValidPlaylistId_ReturnsCorrectSummaries()
    {
        // Arrange
        var playlistId = PlaylistSeeds.MusicPlaylist.Id;
        var expectedEntities = new[]
        {
            PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody,
            PlaylistMultimediaSeeds.MusicPlaylist_AmericanIdiot
        };

        // Act
        var result = await _mediumFacadeSUT.GetMediaByPlaylistIdAsync(playlistId);

        // Assert
        Assert.NotNull(result);
        var resultList = result.ToList();
        Assert.Equal(expectedEntities.Length, resultList.Count);

        foreach (var expected in expectedEntities)
        {
            var summary = resultList.SingleOrDefault(m => m.Id == expected.Id);
            Assert.NotNull(summary);
            DeepAssert.Equal(MediumModelMapper.MapToSummary(expected), summary);
        }
    }

    [Fact]
    public async Task GetMediaByPlaylistIdAsync_UnknownPlaylistId_ReturnsEmptyList()
    {
        // Arrange
        var unknownId = Guid.NewGuid();

        // Act
        var result = await _mediumFacadeSUT.GetMediaByPlaylistIdAsync(unknownId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetMediaByPlaylistIdAsync_ExistingPlaylistWithNoMedia_ReturnsEmptyList()
    {
        // Arrange
        var emptyPlaylistId = PlaylistSeeds.EmptyPlaylist.Id;

        // Act
        var result = await _mediumFacadeSUT.GetMediaByPlaylistIdAsync(emptyPlaylistId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

}
