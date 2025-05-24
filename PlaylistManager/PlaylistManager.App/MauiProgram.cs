using Microsoft.Extensions.Logging;
using PlaylistManager.App.Views;
using PlaylistManager.App.ViewModels;
using PlaylistManager.App.Services;


namespace PlaylistManager.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddTransient<SelectManagerView>();
        builder.Services.AddTransient<SelectManagerViewModel>();



#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
