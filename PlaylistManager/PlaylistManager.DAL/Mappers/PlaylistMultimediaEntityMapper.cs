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
            // add all the Multimedia fields you want to update here
        }

    }
}
