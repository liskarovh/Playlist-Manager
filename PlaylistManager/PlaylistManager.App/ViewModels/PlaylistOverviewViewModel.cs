using CommunityToolkit.Mvvm.Input;
using PlaylistManager.App.Messages;
using PlaylistManager.App.Services;
using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Models;
using PropertyChanged;

namespace PlaylistManager.App.ViewModels;

[AddINotifyPropertyChangedInterface]
public partial class PlaylistOverviewViewModel(IPlaylistFacade playlistFacade,
                                               INavigationService navigationService,
                                               IMessengerService messengerService)
    : PlaylistBaseViewModel(playlistFacade, navigationService, messengerService)
{

    public bool IsEditMode { get; set; }
    public PlaylistSummaryModel? CurrentlyEditedPlaylist { get; set; }
    public string EditedPlaylistTitle { get; set; } = string.Empty;

    [RelayCommand]
    private async Task CreatePlaylist()
    {
        var savedPlaylist = await CreatePlaylistInternal();
        MessengerService.Send(new PlaylistAddMessage(savedPlaylist));
    }

    [RelayCommand]
    private async Task DeletePlaylist(Guid playlistId)
    {
        await DeletePlaylistInternal(playlistId);
        MessengerService.Send(new PlaylistDeleteMessage(playlistId.ToString()));

    }

    [RelayCommand]
    private void TogglePlaylistSortOrder()
    {
        TogglePlaylistSortOrderInternal();
    }

    [RelayCommand]
    private async Task SelectPlaylist(PlaylistSummaryModel? playlist)
    {
        if (playlist == null) return;

        if (IsEditMode && CurrentlyEditedPlaylist != null && CurrentlyEditedPlaylist != playlist)
        {
            await FinishCurrentEditingAsync();
        }

        if (IsEditMode)
        {
            CurrentlyEditedPlaylist = playlist;
            EditedPlaylistTitle = playlist.Title;
        }
        else
        {
            await NavigationService.GoToAsync("/media");

            MessengerService.Send(new PlaylistDisplayMessage(playlist.PlaylistId, SelectedManagerType));
        }
    }

    [RelayCommand]
    private async Task ToggleEditMode()
    {
        if (IsEditMode && CurrentlyEditedPlaylist != null)
        {
            await FinishCurrentEditingAsync();
        }

        IsEditMode = !IsEditMode;

        if (!IsEditMode)
        {
            CurrentlyEditedPlaylist = null;
            EditedPlaylistTitle = string.Empty;
        }
    }

    [RelayCommand]
    private async Task BackgroundTapped()
    {
        if (IsEditMode && CurrentlyEditedPlaylist != null)
        {
            await FinishCurrentEditingAsync();
        }
    }

    private async Task FinishCurrentEditingAsync()
    {
        if (CurrentlyEditedPlaylist == null) return;

        if (!string.IsNullOrWhiteSpace(EditedPlaylistTitle))
        {
            if (CurrentlyEditedPlaylist.Title != EditedPlaylistTitle)
            {
                await SaveEditedPlaylistAsync();
            }
        }

        CurrentlyEditedPlaylist = null;
        EditedPlaylistTitle = string.Empty;
    }

    [RelayCommand]
    private async Task SaveEditedPlaylistAsync()
    {
        if (CurrentlyEditedPlaylist == null || string.IsNullOrWhiteSpace(EditedPlaylistTitle))
            return;

        var updatedPlaylist = CurrentlyEditedPlaylist with
        {
            Title = EditedPlaylistTitle
        };

        var savedPlaylist = await PlaylistFacade.SaveAsync(updatedPlaylist);

        var index = Playlists.ToList().FindIndex(p => p.PlaylistId == savedPlaylist.PlaylistId);
        if (index >= 0)
            Playlists[index] = savedPlaylist;

        MessengerService.Send(new PlaylistEditMessage(savedPlaylist));
    }

    [RelayCommand]
    private void CancelEdit()
    {
        CurrentlyEditedPlaylist = null;
        EditedPlaylistTitle = string.Empty;
    }


    [RelayCommand]
    private async Task GoBack()
    {
        if (IsEditMode && CurrentlyEditedPlaylist != null)
        {
            await FinishCurrentEditingAsync();
        }

        await NavigationService.GoToAsync("//select");
    }

    [RelayCommand]
    private async Task NavigateToSettings()
    {
        await NavigationService.GoToAsync("/settings");
    }

    [RelayCommand]
    private async Task SavePlaylist(PlaylistSummaryModel updatedPlaylist)
    {
        if (updatedPlaylist == null)
        {
            return;
        }

        await PlaylistFacade.SaveAsync(updatedPlaylist);
    }
}
