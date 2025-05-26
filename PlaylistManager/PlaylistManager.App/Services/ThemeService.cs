namespace PlaylistManager.App.Services;

public class ThemeService : IThemeService
{
    public AppTheme CurrentTheme => Application.Current?.UserAppTheme ?? AppTheme.Unspecified;

    public void ToggleTheme()
    {
        if (Application.Current == null)
            return;

        Application.Current.UserAppTheme = CurrentTheme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
    }
}

