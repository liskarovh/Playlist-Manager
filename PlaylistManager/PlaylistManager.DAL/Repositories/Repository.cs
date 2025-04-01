// Copyright (c).NET Foundation and contributors.All rights reserved.
// Licensed under the MIT license.See LICENSE file in the project root for full license information.

using PlaylistManager.DAL.Entities;
using PlaylistManager.DAL.Mappers;
using Microsoft.EntityFrameworkCore;

namespace PlaylistManager.DAL.Repositories;

public class Repository <TEntity> (
    DbContext dbContext,
    IEntityMapper<TEntity> entityMapper) : IRepository<TEntity> where TEntity : class, IEntity
{
    private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

    public IQueryable<TEntity> Get() => _dbSet;

    public async ValueTask<bool> ExistsAsync(TEntity entity)
        => entity.Id != Guid.Empty
           && await _dbSet.AnyAsync(e => e.Id == entity.Id).ConfigureAwait(false);

    public TEntity Insert(TEntity entity)
        => _dbSet.Add(entity).Entity;

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        TEntity existingEntity = await _dbSet.SingleAsync(e => e.Id == entity.Id).ConfigureAwait(false);
        entityMapper.MapToExistingEntity(existingEntity, entity);
        return existingEntity;
    }

    public async Task DeleteAsync(Guid entityId)
        => _dbSet.Remove(await _dbSet.SingleAsync(i => i.Id == entityId).ConfigureAwait(false));
}
