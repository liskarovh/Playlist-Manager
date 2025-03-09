using PlaylistManager.DAL.Entities;
using PlaylistManager.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace PlaylistManager.Common.Tests.Seeds;

/// <summary>
/// Provides seed data for AudioBook entities for testing purposes.
/// </summary>
public static class AudioBookSeeds
{
    /// <summary>
    /// Represents an empty AudioBook entity.
    /// </summary>
    public static readonly AudioBookEntity EmptyAudioBook = new()
    {
        Id = default,
        Title = default!,
        Description = default,
        Duration = default,
        Author = default,
        ReleaseYear = default,
        Url = default,
        Genre = default,
        Format = default
    };

    /// <summary>
    /// Represents the AudioBook entity for "Ready Player One".
    /// </summary>
    public static readonly AudioBookEntity ReadyPlayerOne = new()
    {
        Id = Guid.Parse("7e78f4a0-b6bf-4816-854b-176359f22ff3"),
        Title = "Ready Player One",
        Description = "Dystopian sci-fi novel set in a virtual reality world.",
        Duration = 49680,
        Author = "Ernest Cline",
        ReleaseYear = 2011,
        Url = "https://isbnsearch.org/isbn/9780307887443",
        Genre = AudioBookGenre.SciFi,
        Format = AudioFormat.Mp3
    };

    /// <summary>
    /// Represents an updated clone of the "Ready Player One" AudioBook entity.
    /// </summary>
    public static readonly AudioBookEntity ReadyPlayerOneUpdate = new()
    {
        Id = Guid.Parse("51ae4321-45d9-4ec1-bd81-1a3ec5d884ea"),
        Title = "Ready Player One",
        Description = "Dystopian sci-fi novel set in a virtual reality world.",
        Duration = 49680,
        Author = "Ernest Cline",
        ReleaseYear = 2011,
        Url = "https://isbnsearch.org/isbn/9780307887443",
        Genre = AudioBookGenre.SciFi,
        Format = AudioFormat.Mp3
    };

    /// <summary>
    /// Represents a clone of the "Ready Player One" AudioBook entity for deletion.
    /// </summary>
    public static readonly AudioBookEntity ReadyPlayerOneDelete = new()
    {
        Id = Guid.Parse("97b5afd8-e2d2-430d-b762-401bc3f9c382"),
        Title = "Ready Player One",
        Description = "Dystopian sci-fi novel set in a virtual reality world.",
        Duration = 49680,
        Author = "Ernest Cline",
        ReleaseYear = 2011,
        Url = "https://isbnsearch.org/isbn/9780307887443",
        Genre = AudioBookGenre.SciFi,
        Format = AudioFormat.Mp3
    };

    /// <summary>
    /// Represents the AudioBook entity form "Animal Farm".
    /// </summary>
    public static readonly AudioBookEntity AnimalFarm = new()
    {
        Id = Guid.Parse("242ac26b-9b12-4312-a589-f444bbd3c53d"),
        Title = "Animal Farm",
        Description = "Allegorical novella reflecting events leading up to the Russian Revolution.",
        Duration = 37500,
        Author = "George Orwell",
        ReleaseYear = 1945,
        Url = "https://isbnsearch.org/isbn/9780679420392",
        Genre = AudioBookGenre.Dystopia,
        Format = AudioFormat.Wav
    };

    /// <summary>
    /// Represents the AudioBook entity for "Pride and Prejudice".
    /// </summary>
    public static readonly AudioBookEntity PrideAndPrejudice = new()
    {
        Id = Guid.Parse("e6851b7d-2b46-4a2d-b5b4-a9c0701c2495"),
        Title = "Pride and Prejudice",
        Description = "A classic romance novel about love, society, and manners.",
        Duration = 43500,
        Author = "Jane Austen",
        ReleaseYear = 1813,
        Url = "https://isbnsearch.org/isbn/9780141439518",
        Genre = AudioBookGenre.Romance,
        Format = AudioFormat.Flac
    };

    /// <summary>
    /// Represents the AudioBook entity for "Dune".
    /// </summary>
    public static readonly AudioBookEntity Dune = new()
    {
        Id = Guid.Parse("2cb8f9fa-3d38-4156-9d40-0d5f86930916"),
        Title = "Dune",
        Description = "Epic sci-fi saga about politics, religion, and ecology.",
        Duration = 75600,
        Author = "Frank Herbert",
        ReleaseYear = 1965,
        Url = "https://isbnsearch.org/isbn/9780441013593",
        Genre = AudioBookGenre.SciFi,
        Format = AudioFormat.Mp3
    };

    /// <summary>
    /// Represents the AudioBook entity for "Harry Potter and the Philosopher's Stone".
    /// </summary>
    public static readonly AudioBookEntity HarryPotter = new()
    {
        Id = Guid.Parse("c701f707-3e8f-4393-b8ec-9b9e4f26038c"),
        Title = "Harry Potter and the Sorcerer's Stone",
        Description = "The first book in the Harry Potter series, a modern fantasy classic.",
        Duration = 505,
        Author = "J.K. Rowling",
        ReleaseYear = 1997,
        Url = "https://isbnsearch.org/isbn/9781338878929",
        Genre = AudioBookGenre.Fantasy,
        Format = AudioFormat.Aac
    };

    /// <summary>
    /// Seeds the database context with AudioBook entities.
    /// </summary>
    /// <param name="dbx">The database context to seed.</param>
    /// <returns>The seeded database context.</returns>
    public static DbContext SeedAudioBook(this DbContext dbx)
    {
        dbx.Set<AudioBookEntity>()
           .AddRange(HarryPotter,
                     Dune,
                     PrideAndPrejudice,
                     AnimalFarm,
                     ReadyPlayerOne,
                     ReadyPlayerOneUpdate,
                     ReadyPlayerOneDelete
                    );
        return dbx;
    }
}
