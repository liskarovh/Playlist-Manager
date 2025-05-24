using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PlaylistManager.App.Messages;
using PlaylistManager.App.Services;
using PlaylistManager.App.Views;
using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Models;
using PropertyChanged;

namespace PlaylistManager.App.ViewModels;

[AddINotifyPropertyChangedInterface]
public partial class MediumSelectedViewModel : ViewModelBase

{
    public MediumSelectedViewModel(IMessengerService messengerService) : base(messengerService)
    {
    }
}
