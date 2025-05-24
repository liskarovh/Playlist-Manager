// Copyright (c).NET Foundation and contributors.All rights reserved.
// Licensed under the MIT license.See LICENSE file in the project root for full license information.
using PlaylistManager.Common.Enums;
namespace PlaylistManager.App.Messages;

public record ManagerSelectedMessage
{
    public required ManagerType SelectedType { get; init; }
}
