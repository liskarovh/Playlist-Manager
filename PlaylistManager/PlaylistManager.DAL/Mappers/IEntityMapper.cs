using PlaylistManager.DAL.Entities;

namespace PlaylistManager.DAL.Mappers;

public interface IEntityMapper<in TEntity>
    where TEntity : IEntity
{
    void MapToExistingEntity(TEntity existingEntity, TEntity newEntity);
}
