using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PlaylistManager.App.Messages;
using PlaylistManager.App.Services;
using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Models;
using PlaylistManager.Common.Enums;
using PlaylistManager.BL.Enums;
using PropertyChanged;
using PlaylistManager.App.Resources.Texts;
using System.Globalization;

namespace PlaylistManager.App.ViewModels;

[AddINotifyPropertyChangedInterface]
public partial class SettingsViewModel(IThemeService themeService, IMessengerService messengerService, INavigationService navigationService)
    : ViewModelBase(messengerService)
{
    public bool EnglishLanguage = !CultureInfo.CurrentUICulture.Name.Equals("cs-CZ", StringComparison.OrdinalIgnoreCase);
    private readonly IThemeService _themeService = themeService;
    public bool DarkMode = !themeService.CurrentTheme.Equals(AppTheme.Light);
    private readonly INavigationService _navigationService = navigationService;

    [RelayCommand]
    private async Task SwitchLocale ()
    {
        CultureInfo newCulture = EnglishLanguage
            ? new CultureInfo("cs-CZ")
            : new CultureInfo("en-US");
        CultureInfo.CurrentCulture = newCulture;
        CultureInfo.CurrentUICulture = newCulture;
        AppTexts.Culture = newCulture;

        EnglishLanguage = !EnglishLanguage;

        await Task.CompletedTask;
    }

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
