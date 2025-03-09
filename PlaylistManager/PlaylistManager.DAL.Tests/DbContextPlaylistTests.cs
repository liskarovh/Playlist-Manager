using PlaylistManager.Common.Enums;
using PlaylistManager.Common.Tests;
using PlaylistManager.Common.Tests.Seeds;
using PlaylistManager.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace PlaylistManager.DAL.Tests;

/// <summary>
/// Test class for DbContext operations related to playlists.
/// </summary>
public class DbContextPlaylistTests(ITestOutputHelper output) : DbContextTestsBase(output)
{
    /// <summary>
    /// Tests adding a new playlist without multimedia and persisting it.
    /// </summary>
    [Fact]
    public async Task AddNew_PlaylistWithoutPlaylistMultimedia_Persisted()
    {
        // Arrange
        var entity = PlaylistSeeds.EmptyPlaylist with
        {
            Title = "Favourites",
            Description = "My liked music.",
        };

        // Act
        PlaylistManagerDbContextSUT.Playlists.Add(entity);
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actualEntity = await dbx.Playlists
                                    .SingleAsync(i => i.Id == entity.Id);
        DeepAssert.Equal(entity, actualEntity);
    }

    /// <summary>
    /// Tests adding a new playlist with multimedia and persisting it.
    /// </summary>
    [Fact]
    public async Task AddNew_PlaylistWithMultimedia_Persisted()
    {
        // Arrange
        var entity = PlaylistSeeds.EmptyPlaylist with
        {
            Title = "My playlist",
            Description = "Simple playlist",
            PlaylistMultimedia = new List<PlaylistMultimediaEntity>
            {
                PlaylistMultimediaSeeds.EmptyPlaylistMultimedia with
                {
                    Multimedia = MusicSeeds.EmptyMusic with
                    {
                        Title = "Watching The Wheels",
                        Description = "John Lennon embraces a peaceful life away from fame, despite others’ confusion.",
                        Duration = 355,
                        Author = "John Lennon",
                        ReleaseYear = 1981,
                        Url = "https://music.youtube.com/watch?v=2Kx2PbA8bCI",
                        Genre = MusicGenre.Rock,
                        Format = AudioFormat.None
                    }
                },
            }
        };

        // Act
        PlaylistManagerDbContextSUT.Playlists.Add(entity);
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actualEntity = await dbx.Playlists
                                    .Include(i => i.PlaylistMultimedia)
                                    .ThenInclude(i => i.Multimedia)
                                    .SingleAsync(i => i.Id == entity.Id);
        DeepAssert.Equal(entity, actualEntity);
    }

    /// <summary>
    /// Tests adding a new playlist with just playlist multimedia and persisting it.
    /// </summary>
    [Fact]
    public async Task AddNew_PlaylistWithJustPlaylistMultimedia_Persisted()
    {
        // Arrange
        var entity = PlaylistSeeds.EmptyPlaylist with
        {
            Title = "My playlist",
            Description = "Simple playlist",
            PlaylistMultimedia = new List<PlaylistMultimediaEntity>
            {
                PlaylistMultimediaSeeds.EmptyPlaylistMultimedia with
                {
                    MultimediaId = MusicSeeds.AmericanIdiot.Id,
                },
                PlaylistMultimediaSeeds.EmptyPlaylistMultimedia with
                {
                    MultimediaId = MusicSeeds.BohemianRhapsody.Id,
                }
            }
        };

        // Act
        PlaylistManagerDbContextSUT.Playlists.Add(entity);
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actualEntity = await dbx.Playlists
                                    .Include(i => i.PlaylistMultimedia)
                                    .SingleAsync(i => i.Id == entity.Id);
        DeepAssert.Equal(entity, actualEntity);
    }

    /// <summary>
    /// Tests retrieving all playlists and checking if it contains a seeded playlist.
    /// </summary>
    [Fact]
    public async Task GetAll_Playlists_ContainsSeededPlaylist()
    {
        // Act
        var entities = await PlaylistManagerDbContextSUT.Playlists.ToListAsync();

        // Assert
        DeepAssert.Contains(PlaylistSeeds.MusicPlaylist, entities,
                            nameof(PlaylistEntity.PlaylistMultimedia)
                           );
    }

    /// <summary>
    /// Tests retrieving a playlist by its ID.
    /// </summary>
    [Fact]
    public async Task GetById_Playlist()
    {
        // Act
        var entity = await PlaylistManagerDbContextSUT.Playlists
                                                      .SingleAsync(i => i.Id == PlaylistSeeds.MusicPlaylist.Id);

        // Assert
        DeepAssert.Equal(PlaylistSeeds.MusicPlaylist with
                         {
                             PlaylistMultimedia = Array.Empty<PlaylistMultimediaEntity>()
                         }, entity
                        );
    }

    /// <summary>
    /// Tests retrieving a playlist by its ID including multimedia.
    /// </summary>
    [Fact]
    public async Task GetById_IncludingMultimedia_Playlist()
    {
        // Act
        var entity = await PlaylistManagerDbContextSUT.Playlists
                                                      .Include(i => i.PlaylistMultimedia)
                                                      .ThenInclude(i => i.Multimedia)
                                                      .SingleAsync(i => i.Id == PlaylistSeeds.MusicPlaylist.Id);

        // Assert
        DeepAssert.Equal(PlaylistSeeds.MusicPlaylist, entity);
    }

    /// <summary>
    /// Tests updating a playlist and persisting the changes.
    /// </summary>
    [Fact]
    public async Task Update_Playlist_Persisted()
    {
        // Arrange
        var baseEntity = PlaylistSeeds.MusicPlaylistUpdate;
        var entity =
            baseEntity with
            {
                Title = baseEntity.Title + "Updated",
                Description = baseEntity.Description + "Updated",
            };

        // Act
        PlaylistManagerDbContextSUT.Playlists.Update(entity);
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        // Assert
        await using var dbx = await DbContextFactory.CreateDbContextAsync();
        var actualEntity = await dbx.Playlists.SingleAsync(i => i.Id == entity.Id);
        DeepAssert.Equal(entity, actualEntity);
    }

    /// <summary>
    /// Tests deleting a music playlist without multimedia.
    /// </summary>
    [Fact]
    public async Task Delete_MusicPlaylistWithoutMultimedia_Deleted()
    {
        // Arrange
        var baseEntity = PlaylistSeeds.MusicPlaylistForMultimediaDelete;

        // Act
        PlaylistManagerDbContextSUT.Playlists.Remove(baseEntity);
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        // Assert
        Assert.False(await PlaylistManagerDbContextSUT.Playlists.AnyAsync(i => i.Id == baseEntity.Id));
    }

    /// <summary>
    /// Tests deleting a music playlist by its ID without multimedia.
    /// </summary>
    [Fact]
    public async Task DeleteById_MusicPlaylistWithoutMultimedia_Deleted()
    {
        // Arrange
        var baseEntity = PlaylistSeeds.MusicPlaylistForMultimediaDelete;

        // Act
        PlaylistManagerDbContextSUT.Remove(PlaylistManagerDbContextSUT.Playlists.Single(i => i.Id == baseEntity.Id));
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        // Assert
        Assert.False(await PlaylistManagerDbContextSUT.Playlists.AnyAsync(i => i.Id == baseEntity.Id));
    }

    /// <summary>
    /// Tests deleting a music playlist with multimedia.
    /// </summary>
    [Fact]
    public async Task Delete_MusicPlaylistWithMultimedia_Deleted()
    {
        // Arrange
        var baseEntity = PlaylistSeeds.MusicPlaylistForMultimediaDelete;

        // Act
        PlaylistManagerDbContextSUT.Playlists.Remove(baseEntity);
        await PlaylistManagerDbContextSUT.SaveChangesAsync();

        // Assert
        Assert.False(await PlaylistManagerDbContextSUT.Playlists
                                                      .AnyAsync(i => i.Id == baseEntity.Id)
                    );
        Assert.False(await PlaylistManagerDbContextSUT.PlaylistMultimedia
                                                      .AnyAsync(i => baseEntity.PlaylistMultimedia.Select(ingredientAmount => ingredientAmount.Id).Contains(i.Id))
                    );
    }
}
