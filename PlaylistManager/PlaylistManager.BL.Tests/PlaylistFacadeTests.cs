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
using PlaylistManager.DAL;


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
    public async Task GetPlaylistsByType_Music_ReturnsOnlyMusicPlaylists()
    {
        // Arrange
        PlaylistType playlistType = PlaylistType.Music;

        await using PlaylistManagerDbContext dbContext = await DbContextFactory.CreateDbContextAsync();
        int expectedCount = await dbContext.Playlists
                                           .CountAsync(p => p.Type == playlistType);

        // Act
        IEnumerable<PlaylistSummaryModel> returnedPlaylists = await _facadeSUT.GetPlaylistsByTypeAsync(playlistType);

        // Assert
        IEnumerable<PlaylistSummaryModel> playlistSummaryModels = returnedPlaylists.ToList();
        Assert.Equal(expectedCount, playlistSummaryModels.Count());
        Assert.All(playlistSummaryModels, playlist => Assert.Equal(playlistType, playlist.Type));

        var musicPlaylistModel = PlaylistModelMapper.MapToSummary(PlaylistSeeds.MusicPlaylist);
        Assert.Contains(playlistSummaryModels, p => p.PlaylistId == musicPlaylistModel.PlaylistId);
    }

    [Fact]
    public async Task GetPlaylistsByType_NoPlaylists_ReturnsEmptyCollection()
    {
        // Arrange
        await _facadeSUT.DeleteAsync(PlaylistSeeds.AudioBookPlaylist.Id);

        const PlaylistType playlistType = PlaylistType.AudioBook;

        // Act
        IEnumerable<PlaylistSummaryModel> returnedPlaylists = await _facadeSUT.GetPlaylistsByTypeAsync(playlistType);

        // Assert
        Assert.Empty(returnedPlaylists);
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

    [Fact]
    public async Task GetMediaInPlaylistSortedAsync_PlaylistExists_SortByTitleAscending_ReturnsSortedMedia()
    {
        // Arrange
        var playlistId = PlaylistSeeds.MusicPlaylist.Id;
        var expectedSortedMedia = await GetExpectedSortedMediaInPlaylistAsync(
            playlistId,
            MediaSortBy.Title,
            SortOrder.Ascending
        );

        // Act
        var results = await _facadeSUT.GetMediaInPlaylistSortedAsync(
            playlistId,
            MediaSortBy.Title,
            SortOrder.Ascending
        );

        // Assert
        Assert.NotNull(results);
        DeepAssert.Equal(expectedSortedMedia, results.ToList());
    }

    [Fact]
    public async Task GetMediaInPlaylistSortedAsync_PlaylistExists_SortByAuthorDescending_ReturnsSortedMedia()
    {
        // Arrange
        var playlistId = PlaylistSeeds.MusicPlaylist.Id;
        var expectedSortedMedia = await GetExpectedSortedMediaInPlaylistAsync(
            playlistId,
            MediaSortBy.Author,
            SortOrder.Descending
        );

        // Act
        var results = await _facadeSUT.GetMediaInPlaylistSortedAsync(
            playlistId,
            MediaSortBy.Author,
            SortOrder.Descending
        );

        // Assert
        Assert.NotNull(results);
        DeepAssert.Equal(expectedSortedMedia, results.ToList());
    }

    [Fact]
    public async Task GetMediaInPlaylistSortedAsync_PlaylistExists_SortByDurationAscending_ReturnsSortedMedia()
    {
        // Arrange
        var playlistId = PlaylistSeeds.MusicPlaylist.Id;
        var expectedSortedMedia = await GetExpectedSortedMediaInPlaylistAsync(
            playlistId,
            MediaSortBy.Duration,
            SortOrder.Ascending
        );

        // Act
        var results = await _facadeSUT.GetMediaInPlaylistSortedAsync(
            playlistId,
            MediaSortBy.Duration,
            SortOrder.Ascending
        );

        // Assert
        Assert.NotNull(results);
        DeepAssert.Equal(expectedSortedMedia, results.ToList());
    }

    [Fact]
    public async Task GetMediaInPlaylistSortedAsync_PlaylistDoesNotExist_ReturnsEmpty()
    {
        // Arrange
        var nonExistentPlaylistId = Guid.NewGuid();

        // Act
        var results = await _facadeSUT.GetMediaInPlaylistSortedAsync(
            nonExistentPlaylistId,
            MediaSortBy.Title,
            SortOrder.Ascending
        );

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    [Fact]
    public async Task GetMediaInPlaylistSortedAsync_PlaylistExistsButEmpty_ReturnsEmpty()
    {
        // Arrange
        var emptyPlaylistId = PlaylistSeeds.EmptyPlaylist.Id;

        // Act
        var results = await _facadeSUT.GetMediaInPlaylistSortedAsync(
            emptyPlaylistId,
            MediaSortBy.Title,
            SortOrder.Ascending
        );

        // Assert
        Assert.NotNull(results);
        Assert.Empty(results);
    }

    private async Task<List<MediumSummaryModel>> GetExpectedSortedMediaInPlaylistAsync(
        Guid playlistId,
        MediaSortBy sortBy,
        SortOrder sortOrder
    )
    {
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var pmEntities = await dbx.PlaylistMultimedia
            .Include(pm => pm.Multimedia)
            .AsNoTracking()
            .Where(pm => pm.PlaylistId == playlistId)
            .ToListAsync();

        var summaries = MediumModelMapper.MapToSummary(pmEntities).ToList();

        IEnumerable<MediumSummaryModel> orderedSummaries = summaries;

        switch (sortBy)
        {
            case MediaSortBy.Title:
                orderedSummaries = sortOrder == SortOrder.Ascending
                    ? summaries.OrderBy(m => m.Title, StringComparer.OrdinalIgnoreCase)
                    : summaries.OrderByDescending(m => m.Title, StringComparer.OrdinalIgnoreCase);
                break;
            case MediaSortBy.Author:
                orderedSummaries = sortOrder == SortOrder.Ascending
                    ? summaries.OrderBy(m => m.Author, StringComparer.OrdinalIgnoreCase)
                    : summaries.OrderByDescending(m => m.Author, StringComparer.OrdinalIgnoreCase);
                break;
            case MediaSortBy.Duration:
                orderedSummaries = sortOrder == SortOrder.Ascending
                    ? summaries.OrderBy(m => m.Duration ?? 0) // Match facade's null handling
                    : summaries.OrderByDescending(m => m.Duration ?? 0); // Match facade's null handling
                break;
            case MediaSortBy.AddedDate:
                orderedSummaries = sortOrder == SortOrder.Ascending
                    ? summaries.OrderBy(m => m.AddedDate)
                    : summaries.OrderByDescending(m => m.AddedDate);
                break;
            default:
                // Replicate facade's default sort
                orderedSummaries = sortOrder == SortOrder.Ascending
                     ? summaries.OrderBy(m => m.Title, StringComparer.OrdinalIgnoreCase)
                     : summaries.OrderByDescending(m => m.Title, StringComparer.OrdinalIgnoreCase);
                break;
        }

        return orderedSummaries.ToList();
    }

    [Theory]
    [InlineData(MediaSortBy.Title, SortOrder.Ascending, "Bohemian", 1)] // "Bohemian Rhapsody"
    [InlineData(MediaSortBy.Title, SortOrder.Descending, "Bohemian", 1)]
    [InlineData(MediaSortBy.Title, SortOrder.Ascending, "B", 1)] // Should still be "Bohemian Rhapsody"
    [InlineData(MediaSortBy.Title, SortOrder.Ascending, null, 2)]      // All 4 media in MusicPlaylist
    [InlineData(MediaSortBy.Author, SortOrder.Ascending, null, 2)]
    [InlineData(MediaSortBy.Duration, SortOrder.Descending, null, 2)]
    [InlineData(MediaSortBy.AddedDate, SortOrder.Ascending, null, 2)]
    public async Task GetMediaInPlaylistSortedAsync_MusicPlaylist_ReturnsFilteredAndSortedCorrectly(
        MediaSortBy sortBy, SortOrder sortOrder, string? titlePrefix, int expectedCount)
    {
        // Arrange
        var playlistId = PlaylistSeeds.MusicPlaylist.Id; // Has 4 diverse media items

        // Get all media for the playlist to manually filter and sort for expected result
        var allMediaInPlaylist = await GetExpectedMediaSummariesForPlaylistAsync(playlistId);

        IEnumerable<MediumSummaryModel> expectedFilteredMedia = allMediaInPlaylist;
        if (!string.IsNullOrEmpty(titlePrefix))
        {
            expectedFilteredMedia = expectedFilteredMedia.Where(m => m.Title.StartsWith(titlePrefix, StringComparison.OrdinalIgnoreCase));
        }

        IOrderedEnumerable<MediumSummaryModel> expectedSortedMedia;
        switch (sortBy)
        {
            case MediaSortBy.Title:
                expectedSortedMedia = sortOrder == SortOrder.Ascending
                    ? expectedFilteredMedia.OrderBy(m => m.Title, StringComparer.OrdinalIgnoreCase)
                    : expectedFilteredMedia.OrderByDescending(m => m.Title, StringComparer.OrdinalIgnoreCase);
                break;
            case MediaSortBy.Author:
                expectedSortedMedia = sortOrder == SortOrder.Ascending
                    ? expectedFilteredMedia.OrderBy(m => m.Author ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                    : expectedFilteredMedia.OrderByDescending(m => m.Author ?? string.Empty, StringComparer.OrdinalIgnoreCase);
                break;
            case MediaSortBy.Duration:
                expectedSortedMedia = sortOrder == SortOrder.Ascending
                    ? expectedFilteredMedia.OrderBy(m => m.Duration ?? 0)
                    : expectedFilteredMedia.OrderByDescending(m => m.Duration ?? 0);
                break;
            case MediaSortBy.AddedDate:
                expectedSortedMedia = sortOrder == SortOrder.Ascending
                    ? expectedFilteredMedia.OrderBy(m => m.AddedDate)
                    : expectedFilteredMedia.OrderByDescending(m => m.AddedDate);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(sortBy));
        }
        var expectedResultList = expectedSortedMedia.ToList();

        // Act
        var results = (await _facadeSUT.GetMediaInPlaylistSortedAsync(playlistId, titlePrefix, sortBy, sortOrder)).ToList();

        // Assert
        Assert.Equal(expectedCount, results.Count);
        Assert.Equal(expectedResultList.Count, results.Count); // Double check count from manual filter/sort

        for (int i = 0; i < expectedResultList.Count; i++)
        {
            Assert.Equal(expectedResultList[i].Id, results[i].Id);
            // For debugging:
            // Console.WriteLine($"Expected: {expectedResultList[i].Title} ({GetSortValue(expectedResultList[i], sortBy)}), Actual: {results[i].Title} ({GetSortValue(results[i], sortBy)})");
        }
    }

    private async Task<List<MediumSummaryModel>> GetExpectedMediaSummariesForPlaylistAsync(Guid playlistId)
    {
        var summaries = new List<MediumSummaryModel>();
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var pmEntities = await dbx.PlaylistMultimedia
            .Where(pm => pm.PlaylistId == playlistId)
            .Include(pm => pm.Multimedia)
            .AsNoTracking()
            .ToListAsync();

        foreach (var pmEntity in pmEntities)
        {
            summaries.Add(MediumModelMapper.MapToSummary(pmEntity));
        }
        return summaries;
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

    private async Task<PlaylistSummaryModel> GetExpectedSummaryModelAsync(Guid playlistId)
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
