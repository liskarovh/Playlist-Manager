// Copyright (c).NET Foundation and contributors.All rights reserved.
// Licensed under the MIT license.See LICENSE file in the project root for full license information.

using PlaylistManager.BL.Models;
using PlaylistManager.DAL.Entities;
namespace PlaylistManager.BL.Facades.Interfaces;

//playlist summarry model should be detailed enough for both listing and editing
//POSSIBLE ISSUE - idk how to fix
public interface IPlaylistFacade : IFacade<PlaylistEntity, PlaylistNameOnlyModel, PlaylistSummaryModel, PlaylistSummaryModel>
{
}
