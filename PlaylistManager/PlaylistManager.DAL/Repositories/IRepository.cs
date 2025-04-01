// Copyright (c).NET Foundation and contributors.All rights reserved.
// Licensed under the MIT license.See LICENSE file in the project root for full license information.

namespace PlaylistManager.DAL.Repositories;
using PlaylistManager.DAL.Entities;

public interface IRepository <TEntity> where TEntity : class, IEntity
{
    IQueryable<TEntity> Get();
    Task DeleteAsync(Guid entityId);
    ValueTask<bool> ExistsAsync(TEntity entity);
    TEntity Insert(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
}
