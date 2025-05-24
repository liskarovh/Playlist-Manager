using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using PlaylistManager.BL.Enums;
using Xunit.Abstractions;
using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Facades;
using PlaylistManager.BL.Models;
using PlaylistManager.Common.Enums;
using PlaylistManager.Common.Tests;
using PlaylistManager.Common.Tests.Seeds;

namespace PlaylistManager.BL.Tests;

public class PlaylistFacadeTests : FacadeTestsBase
{
    private readonly IPlaylistFacade _facadeSUT;

    public PlaylistFacadeTests(ITestOutputHelper output) : base(output)
    {
        _facadeSUT = new PlaylistFacade(UnitOfWorkFactory, PlaylistModelMapper, MediumModelMapper);
    }

    [Fact]
    public async Task Create_WithWithoutMedium_EqualsCreated()
    {
        // Arrange
        var model = new PlaylistSummaryModel()
        {
            Id = Guid.NewGuid(),
            PlaylistId = Guid.NewGuid(),
            Title = "Test Title",
            MediaCount = 0,
            TotalDuration = 0,
            Description = "Test Description"
        };

        // Act
        var returnedModel = await _facadeSUT.SaveAsync(model);

        // Assert
        FixIds(model, returnedModel);
        DeepAssert.Equal(model, returnedModel);
    }

    [Fact]
    public async Task Create_WithNonExistingMedium_Throws()
    {
        // Arrange
        var model = new PlaylistSummaryModel()
        {
            Id = Guid.NewGuid(),
            PlaylistId = Guid.NewGuid(),
            Title = "Test Title",
            Description = "Test Description",
            Medias = new ObservableCollection<MediumDetailedModel>
            {
                new()
                {
                    Id = Guid.Empty,
                    MediumId = Guid.Empty,
                    PlaylistId = Guid.Empty,
                    Title = "Title 1",
                    Format = AudioFormat.Mp3.ToString(),
                    Genre = MusicGenre.Pop.ToString(),
                }
            }
        };

        // Act & Assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(model));
    }

    [Fact]
    public async Task Create_WithExistingMedium_Throws()
    {
        // Arrange
        var model = new PlaylistSummaryModel
        {
            Id = Guid.NewGuid(),
            PlaylistId = Guid.NewGuid(),
            Title = "Test Title",
            Description = "Test Description",
            Medias = new ObservableCollection<MediumDetailedModel>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    MediumId = MusicSeeds.AmericanIdiot.Id,
                    PlaylistId = Guid.Empty,
                    Title = MusicSeeds.AmericanIdiot.Title,
                    Format = MusicSeeds.AmericanIdiot.Format.ToString(),
                    Genre = MusicSeeds.AmericanIdiot.Genre.ToString(),
                }
            }
        };

        // Act & Assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(model));
    }

    [Fact]
    public async Task Create_WithExistingAndNotExistingMedium_Throws()
    {
        // Arrange
        var model = new PlaylistSummaryModel
        {
            Id = Guid.NewGuid(),
            PlaylistId = Guid.NewGuid(),
            Title = "Test Title",
            Description = "Test Description",
            Medias = new ObservableCollection<MediumDetailedModel>
            {
                new()
                {
                    Id = Guid.Empty,
                    MediumId = Guid.Empty,
                    PlaylistId = Guid.Empty,
                    Title = "Title 1",
                    Format = AudioFormat.Mp3.ToString(),
                    Genre = MusicGenre.Pop.ToString(),
                },
                MediumModelMapper.MapToDetailModel(PlaylistMultimediaSeeds.MusicPlaylist_AmericanIdiot)
            }
        };

        // Act & Assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(model));
    }

    [Fact]
    public async Task GetById_FromSeeded_EqualsSeeded()
    {
        // Arrange
        var detailModel = PlaylistModelMapper.MapToDetailModel(PlaylistSeeds.MusicPlaylist);

        // Act
        var returnedModel = await _facadeSUT.GetAsync(detailModel.Id);

        // Assert
        DeepAssert.Equal(detailModel, returnedModel);
    }

    [Fact]
    public async Task GetAll_FromSeeded_ContainsSeeded()
    {
        // Arrange
        var listModel = PlaylistModelMapper.MapToNameOnly(PlaylistSeeds.MusicPlaylist);

        // Act
        var returnedModel = await _facadeSUT.GetAsync();

        // Assert
        Assert.Contains(listModel, returnedModel);
    }

    [Fact]
    public async Task Update_Name_FromSeeded_Updated()
    {
        // Arrange
        var detailModel = PlaylistModelMapper.MapToDetailModel(PlaylistSeeds.MusicPlaylist);
        detailModel.Title = "Changed playlist title";

        // Act
        await _facadeSUT.SaveAsync(detailModel with { Medias = default! });

        // Assert
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();
        var entity = await dbContext.Playlists
            .Include(p => p.PlaylistMultimedia)
            .ThenInclude(pm => pm.Multimedia)
            .SingleAsync(p => p.Id == detailModel.Id);

        var returnedModel = PlaylistModelMapper.MapToDetailModel(entity);

        DeepAssert.Equal(detailModel, returnedModel);
    }

    [Fact]
    public async Task Update_RemoveMedium_FromSeeded_NotUpdated()
    {
        // Arrange
        var detailModel = PlaylistModelMapper.MapToDetailModel(PlaylistSeeds.MusicPlaylist);
        detailModel.Medias.Clear();

        // Act
        await _facadeSUT.SaveAsync(detailModel);

        // Assert
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();
        var entity = await dbContext.Playlists
            .Include(p => p.PlaylistMultimedia)
            .ThenInclude(pm => pm.Multimedia)
            .SingleAsync(p => p.Id == detailModel.Id);

        var returnedModel = PlaylistModelMapper.MapToDetailModel(entity);
        DeepAssert.Equal(PlaylistModelMapper.MapToDetailModel(PlaylistSeeds.MusicPlaylist), returnedModel);
    }

    [Fact]
    public async Task Update_RemoveOneOfMedium_FromSeeded_Updated()
    {
        // Arrange
        var detailModel = PlaylistModelMapper.MapToDetailModel(PlaylistSeeds.MusicPlaylist);
        detailModel.Medias.Remove(detailModel.Medias.First());

        // Act
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(detailModel));

        // Assert
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();
        var entity = await dbContext.Playlists
            .Include(p => p.PlaylistMultimedia)
            .ThenInclude(pm => pm.Multimedia)
            .SingleAsync(p => p.Id == detailModel.Id);

        var returnedModel = PlaylistModelMapper.MapToDetailModel(entity);
        DeepAssert.Equal(PlaylistModelMapper.MapToDetailModel(PlaylistSeeds.MusicPlaylist), returnedModel);
    }

    [Fact]
    public async Task DeleteById_FromSeeded_DoesNotThrow()
    {
        // Arrange & Act & Assert
        await _facadeSUT.DeleteAsync(PlaylistSeeds.MusicPlaylist.Id);
    }

    [Fact]
    public async Task GetPlaylistsByNameAsync_NoMatch_ReturnsEmptyCollection()
    {
        // Arrange
        var prefix = "NonExistentNameXYZ123";

        // Act
        var results = await _facadeSUT.GetPlaylistsByNameAsync(prefix);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }



    [Fact]
    public async Task GetPlaylistsByNameAsync_PrefixMatchSpecific_ReturnsMusicPlaylist()
    {
        // Arrange
        var prefix = "Favorite Movies";
        var expectedPlaylistSeed = PlaylistSeeds.VideoPlaylist;
        var expectedSummary = await GetExpectedSummaryModelAsync(expectedPlaylistSeed.Id);

        // Act
        var results = await _facadeSUT.GetPlaylistsByNameAsync(prefix);

        // Assert
        Assert.NotNull(results);
        var resultList = results.ToList();
        // Assert.Single(resultList);
        DeepAssert.Equal(expectedSummary, resultList.First());
    }

    [Fact]
    public async Task GetPlaylistsByNameAsync_PrefixMatch_ReturnsMatchingPlaylists()
    {
        // Arrange
        // Seeded titles: "Music Playlist", "AudioBook Playlist", "Video Playlist", "Empty Playlist", "Playlist For Delete", "Playlist For Update"
        var prefix = "Music"; // This prefix should match "Playlist For Delete" and "Playlist For Update"
        var expectedPlaylistsSeeds = new[]
            {
                PlaylistSeeds.MusicPlaylist,
                PlaylistSeeds.MusicPlaylistDelete,
                PlaylistSeeds.MusicPlaylistForMultimediaDelete,
                PlaylistSeeds.MusicPlaylistForMultimediaUpdate,
                PlaylistSeeds.MusicPlaylistUpdate
            }
            .Where(p => p.Title.StartsWith(prefix)) // Redundant here as manually selected, but good for clarity
            .ToList();


        var expectedSummaries = new List<PlaylistSummaryModel>();
        foreach (var seed in expectedPlaylistsSeeds)
        {
            expectedSummaries.Add(await GetExpectedSummaryModelAsync(seed.Id));
        }

        // Act
        var results = await _facadeSUT.GetPlaylistsByNameAsync(prefix);

        // Assert
        Assert.NotNull(results);
        var resultList = results.ToList();
        Assert.Equal(expectedPlaylistsSeeds.Count, resultList.Count);

        foreach (var expectedSummary in expectedSummaries)
        {
            Assert.Contains(
                resultList,
                r => r.Id == expectedSummary.Id && r.Title == expectedSummary.Title
            );
            var actualSummary = resultList.Single(r => r.Id == expectedSummary.Id);
            DeepAssert.Equal(expectedSummary, actualSummary);
        }
    }

    [Fact]
    public async Task GetPlaylistsByNameAsync_EmptyPrefix_ReturnsAllPlaylists()
    {
        // Arrange
        var allSeededPlaylists = new[]
        {

            PlaylistSeeds.VideoPlaylist,
            PlaylistSeeds.MusicPlaylist,
            PlaylistSeeds.MusicPlaylistForMultimediaUpdate,
            PlaylistSeeds.MusicPlaylistUpdate,
            PlaylistSeeds.MusicPlaylistForMultimediaDelete,
            PlaylistSeeds.MusicPlaylistDelete,
            PlaylistSeeds.AudioBookPlaylist,
        };
        var expectedSummaries = new List<PlaylistSummaryModel>();
        foreach (var seed in allSeededPlaylists)
        {
            expectedSummaries.Add(await GetExpectedSummaryModelAsync(seed.Id));
        }

        // Act
        var results = await _facadeSUT.GetPlaylistsByNameAsync(string.Empty);

        // Assert
        Assert.NotNull(results);
        var resultList = results.ToList();
        Assert.Equal(allSeededPlaylists.Length, resultList.Count);

        foreach (var expectedSummary in expectedSummaries)
        {
            Assert.Contains(resultList, r => r.Id == expectedSummary.Id);
            var actualSummary = resultList.Single(r => r.Id == expectedSummary.Id);
            DeepAssert.Equal(expectedSummary, actualSummary);
        }
    }

    [Fact]
    public async Task GetPlaylistsByNameAsync_NullPrefix_ReturnsAllPlaylists()
    {
        // Arrange
        var allSeededPlaylists = new[]
        {
            PlaylistSeeds.VideoPlaylist,
            PlaylistSeeds.MusicPlaylist,
            PlaylistSeeds.MusicPlaylistForMultimediaUpdate,
            PlaylistSeeds.MusicPlaylistUpdate,
            PlaylistSeeds.MusicPlaylistForMultimediaDelete,
            PlaylistSeeds.MusicPlaylistDelete,
            PlaylistSeeds.AudioBookPlaylist,
        };
        var expectedSummaries = new List<PlaylistSummaryModel>();
        foreach (var seed in allSeededPlaylists)
        {
            expectedSummaries.Add(await GetExpectedSummaryModelAsync(seed.Id));
        }

        // Act
        var results = await _facadeSUT.GetPlaylistsByNameAsync(null);

        // Assert
        Assert.NotNull(results);
        var resultList = results.ToList();
        Assert.Equal(allSeededPlaylists.Length, resultList.Count);
        foreach (var expectedSummary in expectedSummaries)
        {
            Assert.Contains(resultList, r => r.Id == expectedSummary.Id);
            var actualSummary = resultList.Single(r => r.Id == expectedSummary.Id);
            DeepAssert.Equal(expectedSummary, actualSummary);
        }
    }

    [Fact]
    public async Task GetMediaInPlaylistByTitleAsync_PlaylistExists_PrefixMatches_ReturnsFilteredMedia()
    {
        // Arrange
        var playlistId = PlaylistSeeds.MusicPlaylist.Id;
        var bohemianRhapsodyPmSeed = PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody;
        var expectedMediumSummary = await GetExpectedMediumSummaryFromPmIdAsync(bohemianRhapsodyPmSeed.Id);
        var prefix = "Bohemian";

        // Act
        var results = await _facadeSUT.GetMediaInPlaylistByTitleAsync(playlistId, prefix);

        // Assert
        Assert.NotNull(results);
        var resultList = results.ToList();
        Assert.Single(resultList);
        DeepAssert.Equal(expectedMediumSummary, resultList.First());
    }

    [Fact]
    public async Task GetMediaInPlaylistByTitleAsync_PlaylistExists_PrefixNoMatch_ReturnsEmpty()
    {
        // Arrange
        var playlistId = PlaylistSeeds.MusicPlaylist.Id;
        var prefix = "NonExistentMediaTitleInThisPlaylist";

        // Act
        var results = await _facadeSUT.GetMediaInPlaylistByTitleAsync(playlistId, prefix);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public async Task GetMediaInPlaylistByTitleAsync_PlaylistExists_EmptyPrefix_ReturnsAllMediaInPlaylist()
    {
        // Arrange
        var playlistId = PlaylistSeeds.MusicPlaylist.Id;
        var expectedMediaSeeds = new[]
        {
            PlaylistMultimediaSeeds.MusicPlaylist_AmericanIdiot, // Titles: "American Idiot", "Bohemian Rhapsody"
            PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody
        };
        var expectedSummaries = new List<MediumSummaryModel>();
        foreach (var seed in expectedMediaSeeds)
        {
            var summary = await GetExpectedMediumSummaryFromPmIdAsync(seed.Id);
            if(summary != null) expectedSummaries.Add(summary);
        }

        // Act
        var results = await _facadeSUT.GetMediaInPlaylistByTitleAsync(playlistId, string.Empty);

        // Assert
        Assert.NotNull(results);
        var resultList = results.ToList();
        Assert.Equal(expectedMediaSeeds.Length, resultList.Count);
        foreach (var expectedSummary in expectedSummaries.OrderBy(s => s.Title)) // Order to match facade's OrderBy
        {
            Assert.Contains(resultList, r => r.Id == expectedSummary.Id && r.Title == expectedSummary.Title);
        }
    }

    [Fact]
    public async Task GetMediaInPlaylistByTitleAsync_PlaylistExists_NullPrefix_ReturnsAllMediaInPlaylist()
    {
        // Arrange
        var playlistId = PlaylistSeeds.MusicPlaylist.Id;
        var expectedMediaSeeds = new[]
        {
            PlaylistMultimediaSeeds.MusicPlaylist_AmericanIdiot,
            PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody
        };
        var expectedSummaries = new List<MediumSummaryModel>();
        foreach (var seed in expectedMediaSeeds)
        {
            var summary = await GetExpectedMediumSummaryFromPmIdAsync(seed.Id);
            if(summary != null) expectedSummaries.Add(summary);
        }

        // Act
        var results = await _facadeSUT.GetMediaInPlaylistByTitleAsync(playlistId, null);

        // Assert
        Assert.NotNull(results);
        var resultList = results.ToList();
        Assert.Equal(expectedMediaSeeds.Length, resultList.Count);
        foreach (var expectedSummary in expectedSummaries.OrderBy(s => s.Title))
        {
            Assert.Contains(resultList, r => r.Id == expectedSummary.Id && r.Title == expectedSummary.Title);
        }
    }

    [Fact]
    public async Task GetMediaInPlaylistByTitleAsync_PlaylistDoesNotExist_ReturnsEmpty()
    {
        // Arrange
        var nonExistentPlaylistId = Guid.NewGuid();
        var prefix = "AnyTitle";

        // Act
        var results = await _facadeSUT.GetMediaInPlaylistByTitleAsync(nonExistentPlaylistId, prefix);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public async Task GetMediaInPlaylistByTitleAsync_PlaylistExistsButEmpty_ReturnsEmpty()
    {
        // Arrange
        var emptyPlaylistId = PlaylistSeeds.EmptyPlaylist.Id;
        var prefix = "AnyTitle";

        // Act
        var results = await _facadeSUT.GetMediaInPlaylistByTitleAsync(emptyPlaylistId, prefix);

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Theory]
    [InlineData(PlaylistSortBy.Title, SortOrder.Ascending)]
    [InlineData(PlaylistSortBy.Title, SortOrder.Descending)]
    public async Task GetPlaylistsSortedAsync_ByTitle_ReturnsSortedCorrectly(PlaylistSortBy sortBy, SortOrder sortOrder)
    {
        // Arrange
        // Expected order based on PlaylistSeeds titles:
        // "AudioBook Playlist", "Empty Playlist", "Music Playlist", "Playlist For Delete", "Playlist For Update", "Video Playlist"
        var allPlaylists = new List<PlaylistSummaryModel>
        {
            await GetExpectedSummaryModelAsync(PlaylistSeeds.VideoPlaylist.Id),
            await GetExpectedSummaryModelAsync(PlaylistSeeds.MusicPlaylist.Id),
            await GetExpectedSummaryModelAsync(PlaylistSeeds.MusicPlaylistForMultimediaUpdate.Id),
            await GetExpectedSummaryModelAsync(PlaylistSeeds.MusicPlaylistUpdate.Id),
            await GetExpectedSummaryModelAsync(PlaylistSeeds.MusicPlaylistForMultimediaDelete.Id),
            await GetExpectedSummaryModelAsync(PlaylistSeeds.MusicPlaylistDelete.Id),
            await GetExpectedSummaryModelAsync(PlaylistSeeds.AudioBookPlaylist.Id),
        };

        List<PlaylistSummaryModel> expectedOrderedPlaylists;
        if (sortOrder == SortOrder.Ascending)
        {
            expectedOrderedPlaylists = allPlaylists.OrderBy(p => p.Title, StringComparer.OrdinalIgnoreCase).ToList();
        }
        else
        {
            expectedOrderedPlaylists = allPlaylists.OrderByDescending(p => p.Title, StringComparer.OrdinalIgnoreCase).ToList();
        }

        // Act
        var results = (await _facadeSUT.GetPlaylistsSortedAsync(sortBy, sortOrder)).ToList();

        // Assert
        Assert.Equal(expectedOrderedPlaylists.Count, results.Count);
        for (int i = 0; i < expectedOrderedPlaylists.Count; i++)
        {
            Assert.Equal(expectedOrderedPlaylists[i].Id, results[i].Id);
            Assert.Equal(expectedOrderedPlaylists[i].Title, results[i].Title);
        }
    }

    private static void FixIds(PlaylistSummaryModel expectedModel, PlaylistSummaryModel returnedModel)
    {
        returnedModel.Id = expectedModel.Id;
        returnedModel.PlaylistId = expectedModel.PlaylistId;

        foreach (var mediumModel in returnedModel.Medias)
        {
            var mediumDetailModel =
                expectedModel.Medias.FirstOrDefault(m =>
                                                        m.Title == mediumModel.Title
                                                        && m.Url == mediumModel.Url
                                                        && m.Format == mediumModel.Format
                                                        && m.Description == mediumModel.Description
                                                   );
            if (mediumDetailModel != null)
            {
                mediumModel.Id = mediumDetailModel.Id;
                mediumModel.MediumId = mediumDetailModel.MediumId;
            }
        }
    }
    private async Task<MediumSummaryModel?> GetExpectedMediumSummaryFromPmIdAsync(Guid playlistMultimediaId)
    {
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var pmEntity = await dbx.PlaylistMultimedia
            .Include(pm => pm.Multimedia)
            .AsNoTracking()
            .SingleOrDefaultAsync(pm => pm.Id == playlistMultimediaId);

        return pmEntity == null ? null : MediumModelMapper.MapToSummary(pmEntity);
    }

    private async Task<PlaylistSummaryModel> GetExpectedSummaryModelAsync(
        Guid playlistId
    )
    {
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var playlistEntity = await dbx.Playlists.Include(
                p => p.PlaylistMultimedia
            )!
            .ThenInclude(pm => pm.Multimedia)
            .AsNoTracking()
            .SingleOrDefaultAsync(p => p.Id == playlistId);
        return PlaylistModelMapper.MapToSummary(playlistEntity);
    }
}
