using PlaylistManager.DAL.Entities;

namespace PlaylistManager.DAL.Mappers;

public class AudioBookEntityMapper : IEntityMapper<AudioBookEntity>
{
    public void MapToExistingEntity(AudioBookEntity existingEntity, AudioBookEntity newEntity)
    {
        existingEntity.Title = newEntity.Title;
        existingEntity.Description = newEntity.Description;
        existingEntity.Url = newEntity.Url;
        existingEntity.Format = newEntity.Format;
        existingEntity.Genre = newEntity.Genre;
        existingEntity.Author = newEntity.Author;
        existingEntity.Duration = newEntity.Duration;
        existingEntity.ReleaseYear = newEntity.ReleaseYear;
    }
}
