using PlaylistManager.Common.Tests.Seeds;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace PlaylistManager.DAL.Tests;

/// <summary>
/// Test class for DbContext operations related to playlist multimedia.
/// </summary>
public class DbContextPlaylistMultimediaTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    /// <summary>
    /// Tests retrieving all playlist multimedia for a specific playlist.
    /// </summary>
    [Fact]
    public async Task GetAll_PlaylistMultimedia_ForPlaylist()
    {
        // Act
        var playlistMultimedia = await PlaylistManagerDbContextSUT.PlaylistMultimedia
                                                                  .Where(p => p.PlaylistId == PlaylistSeeds.MusicPlaylist.Id)
                                                                  .ToArrayAsync();
        // Assert
        Assert.Contains(PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody with
                        {
                            Multimedia = null!,
                            Playlist = null!
                        }, playlistMultimedia
                       );
        Assert.Contains(PlaylistMultimediaSeeds.MusicPlaylist_AmericanIdiot with
                        {
                            Multimedia = null!,
                            Playlist = null!
                        }, playlistMultimedia
                       );
    }

    /// <summary>
    /// Tests retrieving all playlist multimedia including multimedia details for a specific playlist.
    /// </summary>
    [Fact]
    public async Task GetAll_PlaylistMedia_IncludingMultimedia_ForPlaylist()
    {
        // Act
        var playlistMultimedia = await PlaylistManagerDbContextSUT.PlaylistMultimedia
                                                                  .Where(i => i.PlaylistId == PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody.PlaylistId)
                                                                  .Include(i => i.Multimedia)
                                                                  .ToArrayAsync();
        // Assert
        Assert.Contains(PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsody with
                        {
                            Playlist = null!
                        }, playlistMultimedia
                       );
        Assert.Contains(PlaylistMultimediaSeeds.MusicPlaylist_AmericanIdiot with
                        {
                            Playlist = null!
                        }, playlistMultimedia
                       );
    }

    /// <summary>
    /// Tests updating a playlist multimedia and persisting the changes.
    /// </summary>
    [Fact]
    public async Task Update_PlaylistMultimedia_Persisted()
    {
        // Arrange
        var baseEntity = PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsodyUpdate;
        var entity =
            baseEntity with
            {
                Playlist = null!,
                Multimedia = null!
            };

        // Act
        PlaylistManagerDbContextSUT.PlaylistMultimedia.Update(entity);
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actualEntity = await dbx.PlaylistMultimedia.SingleAsync(i => i.Id == entity.Id);
        Assert.Equal(entity, actualEntity);
    }

    /// <summary>
    /// Tests deleting a playlist multimedia.
    /// </summary>
    [Fact]
    public async Task Delete_PlaylistMultimedia_Deleted()
    {
        // Arrange
        var baseEntity = PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsodyDelete;

        // Act
        PlaylistManagerDbContextSUT.PlaylistMultimedia.Remove(baseEntity);
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        // Assert
        Assert.False(await PlaylistManagerDbContextSUT.PlaylistMultimedia.AnyAsync(i => i.Id == baseEntity.Id));
    }

    /// <summary>
    /// Tests deleting a playlist multimedia by its ID.
    /// </summary>
    [Fact]
    public async Task DeleteById_PlaylistMultimedia_Deleted()
    {
        // Arrange
        var baseEntity = PlaylistMultimediaSeeds.MusicPlaylist_BohemianRhapsodyDelete;

        // Act
        PlaylistManagerDbContextSUT.Remove(PlaylistManagerDbContextSUT.PlaylistMultimedia.Single(i => i.Id == baseEntity.Id));
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        // Assert
        Assert.False(await PlaylistManagerDbContextSUT.PlaylistMultimedia.AnyAsync(i => i.Id == baseEntity.Id));
    }
}
