using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PlaylistManager.App.Messages;
using PlaylistManager.App.Services;
using PlaylistManager.Common.Enums;

namespace PlaylistManager.App.ViewModels;

public partial class SelectManagerViewModel(IMessengerService messengerService, INavigationService navigationService)
    : ViewModelBase(messengerService)
{
    [ObservableProperty]
    private ManagerType selectedManager = ManagerType.NotDecided;

    [RelayCommand]
    private async Task SelectManagerAsync(ManagerType type)
    {
        if (SelectedManager != type)
        {
            SelectedManager = type;

            MessengerService.Send(new ManagerSelectedMessage
            {
                SelectedType = type
            });
        }

        await navigationService.GoToAsync("//select/manager");
    }
}

