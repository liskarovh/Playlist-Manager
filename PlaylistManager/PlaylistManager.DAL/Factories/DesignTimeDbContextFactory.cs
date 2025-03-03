// Copyright (c).NET Foundation and contributors.All rights reserved.
// Licensed under the MIT license.See LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PlaylistManager.DAL.Factories;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<PlaylistManagerDbContext>
{
    private readonly DbContextSqLiteFactory _dbContextSqLiteFactory = new("playlistmanager.db");

    public PlaylistManagerDbContext CreateDbContext(string[] args) => _dbContextSqLiteFactory.CreateDbContext();
}
