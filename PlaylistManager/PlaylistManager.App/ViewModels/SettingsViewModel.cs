using CommunityToolkit.Mvvm.Input;
using PlaylistManager.App.Services;
using PropertyChanged;


namespace PlaylistManager.App.ViewModels;

[AddINotifyPropertyChangedInterface]
public partial class SettingsViewModel(IThemeService themeService, IMessengerService messengerService, INavigationService navigationService)
    : ViewModelBase(messengerService)
{
    private readonly IThemeService _themeService = themeService;
    private readonly INavigationService _navigationService = navigationService;

    [RelayCommand]
    public async Task ToggleTheme()
    {
        _themeService.ToggleTheme();

        await Task.CompletedTask;
    }

    [RelayCommand]
    public async Task SwitchView()
    {
        await _navigationService.GoToAsync("..");
    }
}
