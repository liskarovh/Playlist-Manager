using CommunityToolkit.Mvvm.Messaging;
using PlaylistManager.App.Services;
using PlaylistManager.App.ViewModels;
using PlaylistManager.App.Views;

namespace PlaylistManager.App;

public static class AppInstaller
{
    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton<AppShell>();

        services.AddSingleton<IMessenger>(_ => StrongReferenceMessenger.Default);
        services.AddSingleton<IMessengerService, MessengerService>();

        services.AddSingleton<IAlertService, AlertService>();

        services.Scan(selector => selector
                                  .FromAssemblyOf<App>()
                                  .AddClasses(filter => filter.AssignableTo<ContentPageBase>())
                                  .AsSelf()
                                  .WithTransientLifetime());

        services.Scan(selector => selector
                                  .FromAssemblyOf<App>()
                                  .AddClasses(filter => filter.AssignableTo<IViewModel>())
                                  .AsSelfWithInterfaces()
                                  .WithTransientLifetime());

        services.AddTransient<INavigationService, NavigationService>();

        services.AddSingleton<IThemeService, ThemeService>();

        return services;
    }
}
