// Copyright (c).NET Foundation and contributors.All rights reserved.
// Licensed under the MIT license.See LICENSE file in the project root for full license information.

using PlaylistManager.BL.Models;
using PlaylistManager.DAL.Entities;
namespace PlaylistManager.BL.Facades.Interfaces;

public interface IMediumFacade : IFacade<PlaylistMultimediaEntity,MediumNameOnlyModel, MediumSummaryModel, MediumDetailedModel>
{
}
