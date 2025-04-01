// Copyright (c).NET Foundation and contributors.All rights reserved.
// Licensed under the MIT license.See LICENSE file in the project root for full license information.

namespace PlaylistManager.DAL.Options;

public class DALOptions
{
    public string DatabaseDirectory { get; init; } = string.Empty;
    public string DatabaseName { get; init; } = string.Empty;
    public string DatabaseFilePath => Path.Combine(DatabaseDirectory, DatabaseName);

    /// <summary>
    /// Deletes database before application startup
    /// </summary>
    public bool RecreateDatabaseEachTime { get; init; } = false;

    /// <summary>
    /// Seeds DemoData from DbContext on database creation.
    /// </summary>
    public bool SeedDemoData { get; init; } = false;
}
