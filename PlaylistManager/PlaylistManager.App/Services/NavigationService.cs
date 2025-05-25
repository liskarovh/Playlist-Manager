using PlaylistManager.App.Models;
using PlaylistManager.App.ViewModels;
using PlaylistManager.App.Views;

namespace PlaylistManager.App.Services;

public class NavigationService : INavigationService
{
    public IEnumerable<RouteModel> Routes { get; } = new List<RouteModel>
    {
        new("//select", typeof(SelectManagerView), typeof(SelectManagerViewModel)),
        new("//playlists", typeof(PlaylistOverviewView), typeof(PlaylistOverviewViewModel)),
        new("//media", typeof(PlaylistSelectedView), typeof(PlaylistSelectedViewModel)),
        new("//mediumDetail", typeof(MediumSelectedView), typeof(MediumSelectedViewModel)),
        new("//settings", typeof(SettingsView), typeof(SettingsViewModel))

    };

    public async Task GoToAsync<TViewModel>()
        where TViewModel : ViewModelBase
    {
        var route = GetRouteByViewModel<TViewModel>();
        await Shell.Current.GoToAsync(route);
    }

    public async Task GoToAsync<TViewModel>(IDictionary<string, object?> parameters)
        where TViewModel : ViewModelBase
    {
        var route = GetRouteByViewModel<TViewModel>();
        await Shell.Current.GoToAsync(route, parameters);
    }

    public async Task GoToAsync(string route)
        => await Shell.Current.GoToAsync(route);

    public async Task GoToAsync(string route, IDictionary<string, object?> parameters)
        => await Shell.Current.GoToAsync(route, parameters);

    public bool SendBackButtonPressed()
        => Shell.Current.SendBackButtonPressed();

    private string GetRouteByViewModel<TViewModel>()
        where TViewModel : ViewModelBase
        => Routes.First(route => route.ViewModelType == typeof(TViewModel)).Route;
}
