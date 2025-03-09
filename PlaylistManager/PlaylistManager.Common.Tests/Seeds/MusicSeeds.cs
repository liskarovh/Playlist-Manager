using PlaylistManager.DAL.Entities;
using PlaylistManager.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace PlaylistManager.Common.Tests.Seeds;

/// <summary>
/// Poskytuje seed data pro entity Music pro testovací účely.
/// </summary>
public static class MusicSeeds
{
    /// <summary>
    /// Reprezentuje prázdnou entitu Music.
    /// </summary>
    public static readonly MusicEntity EmptyMusic = new()
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
    /// Reprezentuje entitu Music pro "Bohemian Rhapsody".
    /// </summary>
    public static readonly MusicEntity BohemianRhapsody = new()
    {
        Id = Guid.Parse(input: "621863f8-82f7-4628-9864-46c8efa604f7"),
        Title = "Bohemian Rhapsody",
        Description = "A progressive rock song with operatic sections.",
        Duration = 355,
        Author = "Queen",
        ReleaseYear = 1975,
        Url = "https://youtu.be/fJ9rUzIMcZQ?si=_20Z0J3G5PMaPbZD",
        Genre = MusicGenre.Rock,
        Format = AudioFormat.Mp3
    };

    /// <summary>
    /// Reprezentuje aktualizovanou kopii entity "Bohemian Rhapsody".
    /// </summary>
    public static readonly MusicEntity BohemianRhapsodyUpdate = new()
    {
        Id = Guid.Parse("88f2f2d5-b9b9-4fcb-953a-66d9d7043803"),
        Title = "Bohemian Rhapsody",
        Description = "A progressive rock song with operatic sections.",
        Duration = 355,
        Author = "Queen",
        ReleaseYear = 1975,
        Url = "https://youtu.be/fJ9rUzIMcZQ?si=_20Z0J3G5PMaPbZD",
        Genre = MusicGenre.Rock,
        Format = AudioFormat.Mp3
    };

    /// <summary>
    /// Reprezentuje kopii entity "Bohemian Rhapsody" pro smazání.
    /// </summary>
    public static readonly MusicEntity BohemianRhapsodyDelete = new()
    {
        Id = Guid.Parse("f1e41bc6-f074-4b38-b7d8-40e7cc937b4b"),
        Title = "Bohemian Rhapsody",
        Description = "A progressive rock song with operatic sections.",
        Duration = 355,
        Author = "Queen",
        ReleaseYear = 1975,
        Url = "https://youtu.be/fJ9rUzIMcZQ?si=_20Z0J3G5PMaPbZD",
        Genre = MusicGenre.Rock,
        Format = AudioFormat.Mp3
    };

    /// <summary>
    /// Reprezentuje entitu Music pro "Shape of You".
    /// </summary>
    public static readonly MusicEntity ShapeOfYou = new()
    {
        Id = Guid.Parse(input: "2f468c5e-6323-42ec-adb8-d3074ffe59d5"),
        Title = "Shape of You",
        Description = "A catchy pop song with tropical house influences.",
        Duration = 253,
        Author = "Ed Sheeran",
        ReleaseYear = 2017,
        Url = "https://youtu.be/JGwWNGJdvx8?si=qFmiG_Bll8ZDQ_TJ",
        Genre = MusicGenre.Pop,
        Format = AudioFormat.Flac
    };

    /// <summary>
    /// Reprezentuje entitu Music pro "American Idiot".
    /// </summary>
    public static readonly MusicEntity AmericanIdiot = new()
    {
        Id = Guid.Parse(input: "4223db81-2941-4db2-b927-45cb904ccec5"),
        Title = "American Idiot",
        Description = "A fast-paced punk rock song about anxiety.",
        Duration = 182,
        Author = "Green Day",
        ReleaseYear = 2004,
        Url = "https://youtu.be/Ee_uujKuJMI?si=WVA1xcTJn0vjyk6h",
        Genre = MusicGenre.Punk,
        Format = AudioFormat.Aac
    };

    /// <summary>
    /// Reprezentuje entitu Music pro "A Cruel Angel's Thesis".
    /// </summary>
    public static readonly MusicEntity CruelAngelsThesis = new()
    {
        Id = Guid.Parse(input: "b3b21744-951c-4bb5-83c7-540a12c9b770"),
        Title = "A Cruel Angel's Thesis",
        Description = "Opening theme from Neon Genesis Evangelion, an iconic anime song.",
        Duration = 245,
        Author = "Yoko Takahashi",
        ReleaseYear = 1995,
        Url = "https://youtu.be/o6wtDPVkKqI?si=zEhSrwkBHBWZrb4R",
        Genre = MusicGenre.Anime,
        Format = AudioFormat.Wav
    };

    /// <summary>
    /// Reprezentuje entitu Music pro "Vltava".
    /// </summary>
    public static readonly MusicEntity Vltava = new()
    {
        Id = Guid.Parse(input: "bb918866-b1f3-4eee-b030-12c82dc33623"),
        Title = "Vltava",
        Description = "A symphonic poem from the cycle Má vlast.",
        Duration = 725,
        Author = "Bedřich Smetana",
        ReleaseYear = 1874,
        Url = "https://youtu.be/uI8iTETiSqU?si=7lHkEoggvd4-4ljt",
        Genre = MusicGenre.Classical,
        Format = AudioFormat.Mp3
    };

    /// <summary>
    /// Seeduje databázový kontext s entitami Music.
    /// </summary>
    /// <param name="dbx">Databázový kontext k seedování.</param>
    /// <returns>Seedovaný databázový kontext.</returns>
    public static DbContext SeedMusic(this DbContext dbx)
    {
        dbx.Set<MusicEntity>()
           .AddRange(Vltava,
                     CruelAngelsThesis,
                     AmericanIdiot,
                     ShapeOfYou,
                     BohemianRhapsody,
                     BohemianRhapsodyUpdate,
                     BohemianRhapsodyDelete
                    );
        return dbx;
    }
}
