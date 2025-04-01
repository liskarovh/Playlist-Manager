using PlaylistManager.DAL.Entities;

namespace PlaylistManager.DAL.Mappers;

public class VideoMediaEntityMapper
{
    public void MapToExistingEntity(VideoMediaEntity existingEntity, VideoMediaEntity newEntity)
    {
        existingEntity.Title = newEntity.Title;
        existingEntity.Description = newEntity.Description;
        existingEntity.Resolution = newEntity.Resolution;
        existingEntity.Url = newEntity.Url;
        existingEntity.Format = newEntity.Format;
        existingEntity.Genre = newEntity.Genre;
        existingEntity.Author = newEntity.Author;
        existingEntity.Duration = newEntity.Duration;
        existingEntity.ReleaseYear = newEntity.ReleaseYear;
    }
}
