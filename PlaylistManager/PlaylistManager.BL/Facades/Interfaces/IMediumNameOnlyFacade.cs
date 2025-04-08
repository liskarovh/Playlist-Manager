// Copyright (c).NET Foundation and contributors.All rights reserved.
// Licensed under the MIT license.See LICENSE file in the project root for full license information.

using PlaylistManager.DAL.Entities;
using PlaylistManager.BL.Models;
namespace PlaylistManager.BL.Facades.Interfaces;

public interface IMediumNameOnlyFacade
{
    Task SaveAsync(MediumNameOnlyModel model);
    Task SaveAsync(MediumNameOnlyModel model, Guid relatedEntityId); // např. PlaylistId
    Task DeleteAsync(Guid id);
    Task<IEnumerable<MediumNameOnlyModel>> GetAsync();
}
