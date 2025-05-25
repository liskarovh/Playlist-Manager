using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PlaylistManager.App.Messages;
using PlaylistManager.App.Services;
using PlaylistManager.Common.Enums;
using PropertyChanged;

namespace PlaylistManager.App.ViewModels;

[AddINotifyPropertyChangedInterface]
public partial class SelectManagerViewModel(IMessengerService messengerService, INavigationService navigationService)
    : ViewModelBase(messengerService)
{
    private ManagerType _selectedManager = ManagerType.NotDecided;

    private ManagerType SelectedManager
    {
        get => _selectedManager;
        set => SetProperty(ref _selectedManager, value);
    }

    [RelayCommand]
    private async Task SelectManagerAsync(ManagerType type)
    {
        if (SelectedManager != type)
        {
            SelectedManager = type;
        }

        await navigationService.GoToAsync("/playlists");

        MessengerService.Send(new ManagerSelectedMessage
        {
            SelectedType = type
        });
    }
}

