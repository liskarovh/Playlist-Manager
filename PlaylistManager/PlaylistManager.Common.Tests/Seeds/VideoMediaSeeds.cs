using PlaylistManager.DAL.Entities;
using PlaylistManager.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace PlaylistManager.Common.Tests.Seeds;

/// <summary>
/// Provides seed data for VideoMedia entities for testing purposes.
/// </summary>
public static class VideoMediaSeeds
{
    /// <summary>
    /// Represents an empty VideoMedia entity.
    /// </summary>
    public static readonly VideoMediaEntity EmptyVideoMedia = new()
    {
        Id = default,
        Title = default!,
        Description = default,
        Duration = default,
        Author = default,
        ReleaseYear = default,
        Url = default,
        Format = default,
        Genre = default,
        Resolution = default
    };

    /// <summary>
    /// Represents the VideoMedia entity for "The Matrix".
    /// </summary>
    public static readonly VideoMediaEntity Matrix = new()
    {
        Id = Guid.Parse(input: "6dccdfa6-feab-403a-a6ad-b14b774cdee5"),
        Title = "The Matrix",
        Description = "A cyberpunk sci-fi film exploring simulated reality.",
        Duration = 7860,
        Author = "The Wachowskis",
        ReleaseYear = 1999,
        Url = "https://www.rottentomatoes.com/m/matrix",
        Genre = VideoGenre.SciFi,
        Format = VideoFormat.Mkv
    };

    /// <summary>
    /// Represents an updated copy of the "The Matrix" entity.
    /// </summary>
    public static readonly VideoMediaEntity MatrixUpdate = new()
    {
        Id = Guid.Parse(input: "8a029d15-ac27-4723-8cc7-6e2448838f4b"),
        Title = "The Matrix",
        Description = "A cyberpunk sci-fi film exploring simulated reality.",
        Duration = 7860,
        Author = "The Wachowskis",
        ReleaseYear = 1999,
        Url = "https://www.rottentomatoes.com/m/matrix",
        Genre = VideoGenre.SciFi,
        Format = VideoFormat.Mkv
    };

    /// <summary>
    /// Represents a copy of the "The Matrix" entity for deletion.
    /// </summary>
    public static readonly VideoMediaEntity MatrixDelete = new()
    {
        Id = Guid.Parse(input: "deb291a5-5af0-416c-909a-f06af798458c"),
        Title = "The Matrix",
        Description = "A cyberpunk sci-fi film exploring simulated reality.",
        Duration = 7860,
        Author = "The Wachowskis",
        ReleaseYear = 1999,
        Url = "https://www.rottentomatoes.com/m/matrix",
        Genre = VideoGenre.SciFi,
        Format = VideoFormat.Mkv
    };

    /// <summary>
    /// Represents the VideoMedia entity for "The Lord of the Rings: The Fellowship of the Ring".
    /// </summary>
    public static readonly VideoMediaEntity LordOfTheRings = new()
    {
        Id = Guid.Parse(input: "d0f2afc0-a42f-4c6a-ab52-54dbe0a04b7c"),
        Title = "The Lord of the Rings: The Fellowship of the Ring",
        Description = "Epic fantasy adventure film based on J. R. R. Tolkien's novel.",
        Duration = 10680,
        Author = "Peter Jackson",
        ReleaseYear = 2001,
        Url = "https://www.rottentomatoes.com/m/the_lord_of_the_rings_the_fellowship_of_the_ring",
        Genre = VideoGenre.Fantasy,
        Format = VideoFormat.Mkv
    };

    /// <summary>
    /// Represents the VideoMedia entity for "The Shining".
    /// </summary>
    public static readonly VideoMediaEntity Shining = new()
    {
        Id = Guid.Parse(input: "8252d7e2-584e-4ab6-bb57-c015c921b92d"),
        Title = "The Shining",
        Description = "A psychological horror film based on Stephen King's novel.",
        Duration = 8760,
        Author = "Stanley Kubrick",
        ReleaseYear = 1980,
        Url = "https://www.rottentomatoes.com/m/shining",
        Genre = VideoGenre.Horror,
        Format = VideoFormat.Mp4
    };

    /// <summary>
    /// Represents the VideoMedia entity for "Conclave".
    /// </summary>
    public static readonly VideoMediaEntity Conclave = new()
    {
        Id = Guid.Parse(input: "80bce8f8-ba34-4845-8377-f85d883c9040"),
        Title = "Conclave",
        Description = "A mysterious thriller about the secretive process of electing a new pope.",
        Duration = 7200,
        Author = "Edward Berger",
        ReleaseYear = 2024,
        Url = "https://www.rottentomatoes.com/m/conclave",
        Genre = VideoGenre.Thriller,
        Format = VideoFormat.Mov
    };

    /// <summary>
    /// Represents the VideoMedia entity for "Monty Python and the Holy Grail".
    /// </summary>
    public static readonly VideoMediaEntity MontyPythonHolyGrail = new()
    {
        Id = Guid.Parse(input: "92523359-8d7e-403f-aa02-cfbe6504c6c2"),
        Title = "Monty Python and the Holy Grail",
        Description = "A cult comedy parodying the Arthurian legend.",
        Duration = 5460,
        Author = "Terry Gilliam, Terry Jones",
        ReleaseYear = 1975,
        Url = "https://www.rottentomatoes.com/m/monty_python_and_the_holy_grail",
        Genre = VideoGenre.Comedy,
        Format = VideoFormat.Avi
    };

    /// <summary>
    /// Seeds the database context with VideoMedia entities.
    /// </summary>
    /// <param name="dbx">The database context to seed.</param>
    /// <returns>The seeded database context.</returns>
    public static DbContext SeedVideoMedia(this DbContext dbx)
    {
        dbx.Set<VideoMediaEntity>()
           .AddRange(MontyPythonHolyGrail,
                     Conclave,
                     Shining,
                     LordOfTheRings,
                     Matrix,
                     MatrixUpdate,
                     MatrixDelete
                    );
        return dbx;
    }
}
