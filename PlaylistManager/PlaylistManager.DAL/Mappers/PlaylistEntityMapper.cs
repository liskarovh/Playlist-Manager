using PlaylistManager.DAL.Entities;

namespace PlaylistManager.DAL.Mappers;

public class PlaylistEntityMapper
{
    public void MapToExistingEntity(PlaylistEntity existingEntity, PlaylistEntity newEntity)
    {
        existingEntity.Title = newEntity.Title;
        existingEntity.Description = newEntity.Description;
    }
}
