namespace PlaylistManager.DAL.Options;

public class DALOptions
{
    private string DatabaseDirectory { get; init; } = string.Empty;
    private string DatabaseName { get; init; } = string.Empty;
    public string DatabaseFilePath => Path.Combine(DatabaseDirectory, DatabaseName);

    /// <summary>
    /// Deletes database before application startup
    /// </summary>
    public bool RecreateDatabaseEachTime { get; init; } = false;

    public bool SeedDemoData { get; init; } = false;
}
