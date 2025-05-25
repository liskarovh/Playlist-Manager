using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using CommunityToolkit.Maui;
using PlaylistManager.App.Services;
using PlaylistManager.BL;
using PlaylistManager.DAL;
using PlaylistManager.DAL.Migrator;
using PlaylistManager.DAL.Options;
using PlaylistManager.DAL.Seeds;
using PlaylistManager.App.Views;
using PlaylistManager.App.ViewModels;

namespace PlaylistManager.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        builder.Services
               .AddDALServices(GetDALOptions(builder.Configuration))
               .AddAppServices()
               .AddBLServices();

        builder.Services.AddSingleton<AppShell>();
        builder.Services.AddTransient<SelectManagerView>();
        builder.Services.AddTransient<SelectManagerViewModel>();
        builder.Services.AddTransient<PlaylistOverviewView>();
        builder.Services.AddTransient<PlaylistOverviewViewModel>();
        builder.Services.AddTransient<PlaylistSelectedView>();
        builder.Services.AddTransient<PlaylistSelectedViewModel>();
        builder.Services.AddTransient<MediumSelectedView>();
        builder.Services.AddTransient<MediumSelectedViewModel>();
        builder.Services.AddSingleton<AppShell>();

        ConfigureAppSettings(builder);

        var app = builder.Build();

        MigrateDb(app.Services.GetRequiredService<IDbMigrator>());
        SeedDb(app.Services.GetRequiredService<IDbSeeder>());
        RegisterRouting(app.Services.GetRequiredService<INavigationService>());

        return app;
    }

    private static void ConfigureAppSettings(MauiAppBuilder builder)
    {
        var configurationBuilder = new ConfigurationBuilder();

        var assembly = Assembly.GetExecutingAssembly();
        const string appSettingsFilePath = "PlaylistManager.App.appsettings.json";
        using var appSettingsStream = assembly.GetManifestResourceStream(appSettingsFilePath);
        if (appSettingsStream is not null)
        {
            configurationBuilder.AddJsonStream(appSettingsStream);
        }

        var configuration = configurationBuilder.Build();
        builder.Configuration.AddConfiguration(configuration);
    }

    private static void RegisterRouting(INavigationService navigationService)
    {
        foreach (var route in navigationService.Routes)
        {
            Routing.RegisterRoute(route.Route, route.ViewType);
        }
    }

    private static DALOptions GetDALOptions(IConfiguration configuration)
    {
        DALOptions dalOptions = new()
        {
            DatabaseDirectory = FileSystem.AppDataDirectory,
            SeedDemoData = true
        };
        configuration.GetSection("PlaylistManager:DAL").Bind(dalOptions);
        return dalOptions;
    }

    private static void MigrateDb(IDbMigrator migrator) => migrator.Migrate();

    private static void SeedDb(IDbSeeder dbSeeder) => dbSeeder.Seed();
}
