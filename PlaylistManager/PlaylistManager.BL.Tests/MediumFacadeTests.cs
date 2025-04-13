using System;
using PlaylistManager.Common.Enums;
using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Facades;
using PlaylistManager.BL.Mappers;
using PlaylistManager.BL.Models;
using PlaylistManager.Common.Tests;
using PlaylistManager.Common.Tests.Seeds;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace PlaylistManager.BL.Tests;

public class MediumFacadeTests : FacadeTestsBase
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly IMediumFacade _mediumFacade;

    public MediumFacadeTests(ITestOutputHelper output) : base(output)
    {
        _testOutputHelper = output;
        _mediumFacade = new MediumFacade(UnitOfWorkFactory, MediumModelMapper);
    }


    [Fact]
    public async Task GetById_NonExistentMedium_ReturnsNull()
    {
        // Arrange
        var seededMediumInsidePlaylist = PlaylistMultimediaSeeds.EmptyPlaylistMultimedia;

        // Act
        var result = await _mediumFacade.GetAsync(seededMediumInsidePlaylist.Id);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAll_Single_SeededMusicBohemianRhapsodyNameOnly()
    {
        // Arrange
        var seededMediumInsidePlaylist = PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody;

        // Act
        var media = await _mediumFacade.GetAsync();
        var medium = media.SingleOrDefault(m => m.Id == seededMediumInsidePlaylist.Id);

        // Assert
        DeepAssert.Equal(MediumModelMapper.MapToNameOnly(seededMediumInsidePlaylist), medium);
    }

    [Fact]
    public async Task Debug_GetAll_Single_SeededMusicBohemianRhapsodyNameOnly()
    {
        // Arrange
        var seededMediumInsidePlaylist = PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody;

        // Act
        var media = await _mediumFacade.GetAsync();

        // Debugging: ověříme, že seznam není null a obsahuje položky.
        Assert.NotNull(media);
        Assert.NotEmpty(media);

        // Přidání logování: vypiš informace o každém vráceném modelu.
        foreach (var m in media)
        {
            _testOutputHelper.WriteLine($"Found medium: Id={m.Id},MediumId={m.MediumId}, Title={m.Title}, AddedDate={m.AddedDate}");
        }

        // Hledáme seedovanou entitu podle jejího Id.
        var medium = media.SingleOrDefault(m => m.Id == seededMediumInsidePlaylist.Id);

        // Debug: log výsledek hledání
        if (medium == null)
        {
            _testOutputHelper.WriteLine($"Seeded medium with Id {seededMediumInsidePlaylist.Id} not found in returned collection!");
        }
        else
        {
            _testOutputHelper.WriteLine($"Seeded medium found: Id={medium.Id}, Title={medium.Title}");
        }

        Assert.NotNull(medium); // Ověříme, že medium není null

        // Assert: porovnáme očekávaný model s výsledkem (DeepAssert předpokládá, že kontroluje všechny klíčové vlastnosti)
        DeepAssert.Equal(MediumModelMapper.MapToNameOnly(seededMediumInsidePlaylist), medium);

    }

    [Fact]
    public async Task GetById_SeededMusicBohemianRhapsodyDetailed()
    {
        // Arrange
        var seededMediumInsidePlaylist = PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody;

        // Act
        var result = await _mediumFacade.GetAsync(seededMediumInsidePlaylist.Id);

        // Assert
        DeepAssert.Equal(MediumModelMapper.MapToDetailModel(seededMediumInsidePlaylist), result);
    }

    [Fact]
    public async Task Delete_SeededMusicBohemianRhapsody_Deleted()
    {
        // Arrange
        var seededMediumInsidePlaylist = PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody;

        // Act
        await _mediumFacade.DeleteAsync(seededMediumInsidePlaylist.Id);

        // Assert
        await using var dbxAssert = await DbContextFactory.CreateDbContextAsync();
        Assert.False(await dbxAssert.Music.AnyAsync(i => i.Id == seededMediumInsidePlaylist.Id));
    }

    [Fact]
    public async Task Delete_SeededMusicBohemianRhapsody_Throws()
    {
        // Arrange
        var seededMediumInsidePlaylist = PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _mediumFacade.DeleteAsync(seededMediumInsidePlaylist.Id));
    }

    [Fact]
    public async Task NewMedium_InsertOrUpdate_MediumInserted()
    {
        // Arrange
        var medium = new MediumDetailedModel
        {
            Id = PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody.Id,
            MediumId = MusicSeeds.BohemianRhapsody.Id,
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
        medium = await _mediumFacade.SaveAsync(medium);

        // Assert
        await using var dbxAssert = await DbContextFactory.CreateDbContextAsync();
        var mediumFromDb = await dbxAssert.PlaylistMultimedia.SingleAsync(i => i.Id == medium.Id);
        DeepAssert.Equal(medium, MediumModelMapper.MapToDetailModel(mediumFromDb));
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
        await _mediumFacade.SaveAsync(medium);

        // Assert
        await using var dbxAssert = await DbContextFactory.CreateDbContextAsync();
        var mediumFromDb = await dbxAssert.PlaylistMultimedia.SingleAsync(i => i.Id == medium.Id);
        DeepAssert.Equal(medium, MediumModelMapper.MapToDetailModel(mediumFromDb));
    }
}
