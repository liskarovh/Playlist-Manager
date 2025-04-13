using System.Collections.ObjectModel;

using PlaylistManager.Common.Tests;
using PlaylistManager.Common.Tests.Seeds;
using PlaylistManager.BL.Facades;
using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Models;
using PlaylistManager.Common.Enums;
using Xunit.Abstractions;

namespace PlaylistManager.BL.Tests;

public class PlaylistFacadeTests : FacadeTestsBase
{

    private readonly IPlaylistFacade _facadeSUT;

    public PlaylistFacadeTests(ITestOutputHelper output) : base(output)
    {
        _facadeSUT = new PlaylistFacade(UnitOfWorkFactory, PlaylistModelMapper);
    }

    [Fact]
    public async Task Create_WithWithoutMedia_EqualsCreated()
    {
        //Arrange
        // var model = PlaylistSummaryModel.Empty;
        // model.PlaylistId = Guid.NewGuid();
        // model.Title = "Test Title";
        // model.Description = "Test Description";

        var model = new PlaylistSummaryModel()
        {
            Id = Guid.NewGuid(),
            PlaylistId = Guid.NewGuid(),
            Title = "Test Title",
            MediaCount = 0,
            TotalDuration = 0,
            Description = "Test Description"
        };

        //Act
        var returnedModel = await _facadeSUT.SaveAsync(model);

        //Assert
        FixIds(model, returnedModel);
        DeepAssert.Equal(model, returnedModel);
    }

    [Fact]
    public async Task Create_WithNonExistingIngredient_Throws()
    {
        //Arrange
        var model = new PlaylistSummaryModel()
        {
            Id = Guid.NewGuid(),
            PlaylistId = Guid.NewGuid(),
            Title = "Test Title",
            Description = "Test Description",
            Medias = new ObservableCollection<MediumDetailedModel>()
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

        //Act & Assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(model));
    }

    [Fact]
    public async Task Create_WithExistingIngredient_Throws()
    {
        //Arrange
        var model = new PlaylistSummaryModel()
        {
            Id = Guid.NewGuid(),
            PlaylistId = Guid.NewGuid(),
            Title = "Test Title",
            Description = "Test Description",
            Medias = new ObservableCollection<MediumDetailedModel>()
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

        //Act && Assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(model));
    }

    [Fact]
    public async Task Create_WithExistingAndNotExistingIngredient_Throws()
    {
        //Arrange
        var model = new PlaylistSummaryModel()
        {
            Id = Guid.NewGuid(),
            PlaylistId = Guid.NewGuid(),
            Title = "Test Title",
            Description = "Test Description",
            Medias = new ObservableCollection<MediumDetailedModel>()
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

        //Act & Assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _facadeSUT.SaveAsync(model));
    }


    [Fact]
    public async Task GetById_FromSeeded_EqualsSeeded()
    {
        //Arrange
        var detailModel = PlaylistModelMapper.MapToDetailModel(PlaylistSeeds.MusicPlaylist);

        //Act
        var returnedModel = await _facadeSUT.GetAsync(detailModel.Id);

        //Assert
        DeepAssert.Equal(detailModel, returnedModel);
    }

    [Fact]
    public async Task GetAll_FromSeeded_ContainsSeeded()
    {
        //Arrange
        var listModel = PlaylistModelMapper.MapToNameOnly(PlaylistSeeds.MusicPlaylist);

        //Act
        var returnedModel = await _facadeSUT.GetAsync();

        //Assert
        Assert.Contains(listModel, returnedModel);
    }

    [Fact]
    public async Task Update_FromSeeded_DoesNotThrow()
    {
        //Arrange
        var detailModel = PlaylistModelMapper.MapToDetailModel(PlaylistSeeds.MusicPlaylist);
        detailModel.Title = "Changed playlist title";

        //Act & Assert
        await _facadeSUT.SaveAsync(detailModel with {Medias = default!});
    }

    [Fact]
    public async Task Update_Name_FromSeeded_Updated()
    {
        //Arrange
        var detailModel = PlaylistModelMapper.MapToDetailModel(PlaylistSeeds.MusicPlaylist);
        detailModel.Title = "Changed playlist title";

        //Act
        await _facadeSUT.SaveAsync(detailModel with { Medias = default!});

        //Assert
        var returnedModel = await _facadeSUT.GetAsync(detailModel.Id);
        DeepAssert.Equal(detailModel, returnedModel);
    }

    [Fact]
    public async Task Update_RemoveIngredients_FromSeeded_NotUpdated()
    {
        //Arrange
        var detailModel = PlaylistModelMapper.MapToDetailModel(PlaylistSeeds.MusicPlaylist);
        detailModel.Medias.Clear();

        //Act
        await _facadeSUT.SaveAsync(detailModel);

        //Assert
        var returnedModel = await _facadeSUT.GetAsync(detailModel.Id);
        DeepAssert.Equal(PlaylistModelMapper.MapToDetailModel(PlaylistSeeds.MusicPlaylist), returnedModel);
    }

    [Fact]
    public async Task Update_RemoveOneOfIngredients_FromSeeded_Updated()
    {
        //Arrange
        var detailModel = PlaylistModelMapper.MapToDetailModel(PlaylistSeeds.MusicPlaylist);
        detailModel.Medias.Remove(detailModel.Medias.First());

        //Act
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() =>  _facadeSUT.SaveAsync(detailModel));

        //Assert
        var returnedModel = await _facadeSUT.GetAsync(detailModel.Id);
        DeepAssert.Equal(PlaylistModelMapper.MapToDetailModel(PlaylistSeeds.MusicPlaylist), returnedModel);
    }

    [Fact]
    public async Task DeleteById_FromSeeded_DoesNotThrow()
    {
        //Arrange & Act & Assert
        await _facadeSUT.DeleteAsync(PlaylistSeeds.MusicPlaylist.Id);
    }


    private static void FixIds(PlaylistSummaryModel expectedModel, PlaylistSummaryModel returnedModel)
    {
        returnedModel.Id = expectedModel.Id;
        returnedModel.PlaylistId = expectedModel.PlaylistId;

        foreach (var mediumModel in returnedModel.Medias)
        {
            var mediumDetailModel = expectedModel.Medias.FirstOrDefault(m =>
                m.Title == mediumModel.Title
                && m.Url == mediumModel.Url
                && m.Format == mediumModel.Format
                && m.Description == mediumModel.Description);

            if (mediumDetailModel != null)
            {
                mediumModel.Id = mediumDetailModel.Id;
                mediumModel.MediumId = mediumDetailModel.MediumId;
            }
        }
    }
}
