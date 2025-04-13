using Microsoft.EntityFrameworkCore.Query;
using PlaylistManager.DAL.Entities;

namespace PlaylistManager.DAL.Repositories;

public interface IRepository<TEntity> where TEntity : class, IEntity
{
    IQueryable<TEntity> Get(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);
    Task DeleteAsync(Guid entityId);
    ValueTask<bool> ExistsAsync(TEntity entity);
    TEntity Insert(TEntity entity, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);
    Task<TEntity> UpdateAsync(TEntity entity, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);
}
