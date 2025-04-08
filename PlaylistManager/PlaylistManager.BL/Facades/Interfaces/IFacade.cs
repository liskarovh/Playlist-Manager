// Copyright (c).NET Foundation and contributors.All rights reserved.
// Licensed under the MIT license.See LICENSE file in the project root for full license information.

using PlaylistManager.BL.Models;
using PlaylistManager.DAL.Entities;

namespace PlaylistManager.BL.Facades.Interfaces;

public interface IFacade<TEntity, TListModel, TDetailModel>
    where TEntity : class, IEntity
    where TListModel : IModel
    where TDetailModel : class, IModel
{
    Task DeleteAsync(Guid id);
    Task<TDetailModel?> GetAsync(Guid id);
    Task<IEnumerable<TListModel>> GetAsync();
    Task<TDetailModel> SaveAsync(TDetailModel model);
}
