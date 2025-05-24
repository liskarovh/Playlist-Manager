using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlaylistManager.App.Messages;
using PlaylistManager.App.Services;
using PlaylistManager.Common.Enums;
using PropertyChanged;

namespace PlaylistManager.App.ViewModels;

[AddINotifyPropertyChangedInterface]
public partial class SelectManagerViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ManagerType selectedManager = ManagerType.NotDecided;

    public SelectManagerViewModel(IMessengerService messengerService, INavigationService navigationService)
        : base(messengerService)
    {
        _navigationService = navigationService;
    }

    [RelayCommand]
    private async Task SelectManagerAsync(ManagerType type)
    {
        if (SelectedManager != type)
        {
            SelectedManager = type;
        }

        await _navigationService.GoToAsync("//playlists");

        MessengerService.Send(new ManagerSelectedMessage
        {
            SelectedType = type
        });
    }
}

