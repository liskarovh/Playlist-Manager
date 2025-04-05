using PlaylistManager.BL.Models;
using PlaylistManager.Common.Enums;
using PlaylistManager.DAL.Entities;

namespace PlaylistManager.BL.Mappers;

public class MediumModelMapper : ModelMapperBase<PlaylistMultimediaEntity, MediumNameOnlyModel, MediumSummaryModel, MediumDetailedModel>
{
    public override MediumNameOnlyModel MapToNameOnly(PlaylistMultimediaEntity? entity)
        => entity?.Multimedia == null
               ? MediumNameOnlyModel.Empty
               : new MediumNameOnlyModel
               {
                   Id = entity.Id,
                   MediumId = entity.MultimediaId,
                   Title = entity.Multimedia.Title,
                   AddedDate = entity.AddedDate
               };


    public override MediumSummaryModel MapToSummary(PlaylistMultimediaEntity? entity)
        => entity?.Multimedia == null
               ? MediumSummaryModel.Empty
               : new MediumSummaryModel
               {
                   Id = entity.Id,
                   MediumId = entity.MultimediaId,
                   Title = entity.Multimedia.Title,
                   Author = entity.Multimedia.Author,
                   Duration = entity.Multimedia.Duration,
                   AddedDate = entity.AddedDate
               };

    public override MediumDetailedModel MapToDetailModel(PlaylistMultimediaEntity? entity)
    {
        if (entity?.Multimedia == null)
        {
            return MediumDetailedModel.Empty;
        }

        MediumDetailedModel model = new()
        {
            Id = entity.Id,
            MediumId = entity.MultimediaId,
            Title = entity.Multimedia.Title,
            Author = entity.Multimedia.Author,
            Description = entity.Multimedia.Description,
            Url = entity.Multimedia.Url,
            Duration = entity.Multimedia.Duration,
            ReleaseYear = entity.Multimedia.ReleaseYear,
            Format = string.Empty, // Initialized in the switch below
            Genre = string.Empty // Initialized in the switch below
        };

        switch (entity.Multimedia)
        {
            case AudioBookEntity audioBook:
                model.Format = audioBook.Format.ToString();
                model.Genre = audioBook.Genre.ToString();
                break;
            case MusicEntity music:
                model.Format = music.Format.ToString();
                model.Genre = music.Genre.ToString();
                break;
            case VideoMediaEntity video:
                model.Format = video.Format.ToString();
                model.Genre = video.Genre.ToString();
                break;
            case AudioMediaEntity audio:
                model.Format = audio.Format.ToString();
                break;
        }

        return model;
    }

    public override PlaylistMultimediaEntity MapToEntity(MediumDetailedModel model) => throw new InvalidOperationException("This method is unsupported. Use the other overload.");

    public PlaylistMultimediaEntity MapToEntity(MediumDetailedModel model, PlaylistEntity playlistEntity)
    {
        if (model.Id == Guid.Empty)
        {
            throw new ArgumentException("Model Id cannot be empty.", nameof(model.Id));
        }

        if (model.MediumId == Guid.Empty)
        {
            throw new ArgumentException("Medium Id cannot be empty.", nameof(model.MediumId));
        }

        // Create the appropriate multimedia entity based on the format and genre.
        MultimediaBaseEntity multimedia;

        if (Enum.TryParse(model.Format, out AudioFormat audioFormat) && audioFormat != AudioFormat.None)
        {
            if (Enum.TryParse(model.Genre, out AudioBookGenre audioBookGenre) && audioBookGenre != AudioBookGenre.None)
            {
                multimedia = new AudioBookEntity
                {
                    Id = model.MediumId,
                    Title = model.Title,
                    Author = model.Author,
                    Description = model.Description,
                    Url = model.Url,
                    Duration = model.Duration,
                    ReleaseYear = model.ReleaseYear,
                    Format = audioFormat,
                    Genre = audioBookGenre
                };
            }
            else if (Enum.TryParse(model.Genre, out MusicGenre musicGenre) && musicGenre != MusicGenre.None)
            {
                multimedia = new MusicEntity
                {
                    Id = model.MediumId,
                    Title = model.Title,
                    Author = model.Author,
                    Description = model.Description,
                    Url = model.Url,
                    Duration = model.Duration,
                    ReleaseYear = model.ReleaseYear,
                    Format = audioFormat,
                    Genre = musicGenre
                };
            }
            else
            {
                throw new ArgumentException("Invalid genre of audio media.");
            }
        }
        else if (Enum.TryParse(model.Format, out VideoFormat videoFormat) && videoFormat != VideoFormat.None)
        {
            if (Enum.TryParse(model.Genre, out VideoGenre videoGenre) && videoGenre != VideoGenre.None)
            {
                multimedia = new VideoMediaEntity
                {
                    Id = model.MediumId,
                    Title = model.Title,
                    Author = model.Author,
                    Description = model.Description,
                    Url = model.Url,
                    Duration = model.Duration,
                    ReleaseYear = model.ReleaseYear,
                    Format = videoFormat,
                    Genre = videoGenre
                };
            }
            else
            {
                throw new ArgumentException("Invalid genre of video media.");
            }
        }
        else
        {
            throw new ArgumentException("Invalid format of multimedia.");
        }

        // Binds the multimedia entity to the playlist entity.
        return new PlaylistMultimediaEntity
        {
            Id = model.Id,
            PlaylistId = playlistEntity.Id,
            MultimediaId = model.MediumId,
            AddedDate = model.AddedDate,
            Multimedia = multimedia,
            Playlist = playlistEntity
        };
    }
}
