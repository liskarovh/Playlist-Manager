using PlaylistManager.DAL.Entities;

namespace PlaylistManager.DAL.Mappers;

public class MusicEntityMapper : IEntityMapper<MusicEntity>
{
    public void MapToExistingEntity(MusicEntity existingEntity, MusicEntity newEntity)
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
