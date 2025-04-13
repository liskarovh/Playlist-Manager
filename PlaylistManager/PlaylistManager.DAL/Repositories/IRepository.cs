// Copyright (c).NET Foundation and contributors.All rights reserved.
// Licensed under the MIT license.See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore.Query;

namespace PlaylistManager.DAL.Repositories;
using PlaylistManager.DAL.Entities;

public interface IRepository <TEntity> where TEntity : class, IEntity
{
    // IQueryable<TEntity> Get();
    IQueryable<TEntity> Get(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null );
    Task DeleteAsync(Guid entityId);
    ValueTask<bool> ExistsAsync(TEntity entity);
    TEntity Insert(TEntity entity, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);
    Task<TEntity> UpdateAsync(TEntity entity, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);
}
