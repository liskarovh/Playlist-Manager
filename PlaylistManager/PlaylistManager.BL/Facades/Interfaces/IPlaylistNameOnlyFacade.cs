// Copyright (c).NET Foundation and contributors.All rights reserved.
// Licensed under the MIT license.See LICENSE file in the project root for full license information.

using PlaylistManager.BL.Models;
using PlaylistManager.DAL.Entities;
namespace PlaylistManager.BL.Facades;

public interface IPlaylistNameOnlyFacade
{
    Task SaveAsync(PlaylistNameOnlyModel model);
    Task SaveAsync(PlaylistNameOnlyModel model, Guid relatedEntityId); // Optional
    Task DeleteAsync(Guid id);
    Task<IEnumerable<PlaylistNameOnlyModel>> GetAsync();
}
