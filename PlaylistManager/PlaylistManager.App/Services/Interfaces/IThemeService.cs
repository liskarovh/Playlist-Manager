namespace PlaylistManager.App.Services;

public interface IThemeService
{
    AppTheme CurrentTheme { get; }
    void ToggleTheme();
}
