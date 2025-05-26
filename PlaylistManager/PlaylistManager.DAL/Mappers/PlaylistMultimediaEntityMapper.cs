using PlaylistManager.DAL.Entities;

namespace PlaylistManager.DAL.Mappers;

public class PlaylistMultimediaEntityMapper: IEntityMapper<PlaylistMultimediaEntity>
{
    // Currently has nothing to map
    public void MapToExistingEntity(PlaylistMultimediaEntity existingEntity, PlaylistMultimediaEntity newEntity)
    {
        // Map included entity - assuming you want to update its fields
        if (existingEntity.Multimedia != null && newEntity.Multimedia != null)
        {
            existingEntity.Multimedia.Title = newEntity.Multimedia.Title;
            existingEntity.Multimedia.Description = newEntity.Multimedia.Description;
            existingEntity.Multimedia.Author = newEntity.Multimedia.Author;
            existingEntity.Multimedia.Url = newEntity.Multimedia.Url;
            existingEntity.Multimedia.Duration = newEntity.Multimedia.Duration;
            existingEntity.Multimedia.ReleaseYear = newEntity.Multimedia.ReleaseYear;

            if (existingEntity.Multimedia is MusicEntity existingMusic && newEntity.Multimedia is MusicEntity newMusic)
            {
                existingMusic.Format = newMusic.Format;
                existingMusic.Genre = newMusic.Genre;
            }
            else if (existingEntity.Multimedia is AudioBookEntity existingAudioBook && newEntity.Multimedia is AudioBookEntity newAudioBook)
            {
                existingAudioBook.Format = newAudioBook.Format;
                existingAudioBook.Genre = newAudioBook.Genre;
            }
            else if (existingEntity.Multimedia is VideoMediaEntity existingVideo && newEntity.Multimedia is VideoMediaEntity newVideo)
            {
                existingVideo.Format = newVideo.Format;
                existingVideo.Genre = newVideo.Genre;
            }
        }

    }
}
