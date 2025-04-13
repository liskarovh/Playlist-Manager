using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PlaylistManager.DAL.Entities;
using PlaylistManager.DAL.Mappers;

namespace PlaylistManager.DAL.Repositories;

public class Repository<TEntity>(DbContext dbContext, IEntityMapper<TEntity> entityMapper)
    : IRepository<TEntity> where TEntity : class, IEntity
{
    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

    public IQueryable<TEntity> Get(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        IQueryable<TEntity> query = _dbSet;

        if (include != null)
        {
            query = include(query);
        }

        return query;
    }

    public async ValueTask<bool> ExistsAsync(TEntity entity)
        => (entity.Id != Guid.Empty) && await _dbSet.AnyAsync(e => e.Id == entity.Id).ConfigureAwait(false);

    public TEntity Insert(TEntity entity, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        _dbSet.Add(entity);

        // Note: this assumes SaveChanges() is called before you try to reload it.
        if (include == null)
            return entity;

        IQueryable<TEntity> query = _dbSet;
        query = include(query);

        // We assume 'entity.Id' is set correctly either before or after Add() (e.g., with GUIDs or after SaveChanges)
        return query.First(e => e.Id == entity.Id);
    }
    public async Task<TEntity> UpdateAsync(TEntity entity, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
    {
        IQueryable<TEntity> query = _dbSet;

        if (include != null)
        {
            query = include(query);
        }

        TEntity existingEntity = await query.SingleAsync(e => e.Id == entity.Id).ConfigureAwait(false);
        entityMapper.MapToExistingEntity(existingEntity, entity);

        return existingEntity;
    }

    public async Task DeleteAsync(Guid entityId)
        => _dbSet.Remove(await _dbSet.SingleAsync(i => i.Id == entityId).ConfigureAwait(false));
}
