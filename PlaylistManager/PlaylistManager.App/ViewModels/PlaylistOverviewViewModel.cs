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

namespace PlaylistManager.App.ViewModels;

[AddINotifyPropertyChangedInterface]
public partial class PlaylistOverviewViewModel : ViewModelBase,
                                                 IRecipient<PlaylistAddMessage>,
                                                 IRecipient<PlaylistDeleteMessage>,
                                                 IRecipient<PlaylistEditMessage>,
                                                 IRecipient<ManagerSelectedMessage>
{
    private readonly IPlaylistFacade _playlistFacade;
    private readonly INavigationService _navigationService;
    private ManagerType _selectedManagerType = ManagerType.NotDecided;

    public ObservableCollection<PlaylistSummaryModel> Playlists { get; set; } = new();
    public string PlaylistSearchQuery { get; set; } = string.Empty;

    public string SelectedPlaylistSortOption { get; set; } = "Name";
    public SortOrder PlaylistSortOrder { get; set; } = SortOrder.Ascending;
    public string PlaylistSortOrderSymbol { get; set; } = "↑";
    public ObservableCollection<string> PlaylistSortOption { get; } = new(["Name", "Media Count", "Total Duration"]);

    public bool IsEditMode { get; set; } = false;
    public PlaylistSummaryModel? CurrentlyEditedPlaylist { get; set; }
    public string EditedPlaylistTitle { get; set; } = string.Empty;

    public PlaylistOverviewViewModel(IPlaylistFacade playlistFacade, INavigationService navigationService, IMessengerService messengerService)
        : base(messengerService)
    {
        _playlistFacade = playlistFacade;
        _navigationService = navigationService;

        PropertyChanged += async (_, args) =>
                           {
                               switch (args.PropertyName)
                               {
                                   case nameof(PlaylistSearchQuery):
                                       await SearchPlaylistsCommand.ExecuteAsync(PlaylistSearchQuery);
                                       break;
                                   case nameof(SelectedPlaylistSortOption):
                                   case nameof(PlaylistSortOrder):
                                       await SortPlaylistsCommand.ExecuteAsync(SelectedPlaylistSortOption);
                                       break;
                               }
                           };

        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public async void Receive(ManagerSelectedMessage message)
    {
        _selectedManagerType = message.SelectedType;
        await LoadDataAsync();
    }

    public void Receive(PlaylistAddMessage message)
    {
        Playlists.Add(message.Value);
    }

    public void Receive(PlaylistDeleteMessage message)
    {
        var playlist = Playlists.FirstOrDefault(p => p.PlaylistId.ToString() == message.Value);
        if (playlist != null)
            Playlists.Remove(playlist);
    }

    public void Receive(PlaylistEditMessage message)
    {
        var index = Playlists.ToList().FindIndex(p => p.PlaylistId == message.Value.PlaylistId);
        if (index >= 0)
            Playlists[index] = message.Value;
    }

    protected override async Task LoadDataAsync()
    {
        await LoadPlaylists();
    }

    private async Task LoadPlaylists()
    {
        var playlistType = MapManagerTypeToPlaylistType(_selectedManagerType);

        var playlists = await _playlistFacade.GetPlaylistsByTypeAsync(playlistType);

        Playlists.Clear();
        foreach (var playlist in playlists)
        {
            Playlists.Add(playlist);
        }

        await SortPlaylistsCommand.ExecuteAsync(SelectedPlaylistSortOption);
    }

    [RelayCommand]
    private async Task SortPlaylists(string? sortOption)
    {
        if (string.IsNullOrEmpty(sortOption)) return;

        PlaylistSortBy sortBy = sortOption switch
        {
            "Name"           => PlaylistSortBy.Title,
            "Media Count"    => PlaylistSortBy.MediaCount,
            "Total Duration" => PlaylistSortBy.TotalDuration,
            _                => PlaylistSortBy.Title
        };

        var playlistType = MapManagerTypeToPlaylistType(_selectedManagerType);

        var sortedPlaylists = await _playlistFacade.GetPlaylistsSortedAsync(sortBy, PlaylistSortOrder, playlistType);

        Playlists.Clear();
        foreach (var item in sortedPlaylists)
        {
            Playlists.Add(item);
        }
    }

    [RelayCommand]
    private void TogglePlaylistSortOrder()
    {
        PlaylistSortOrder = PlaylistSortOrder == SortOrder.Ascending
                                ? SortOrder.Descending
                                : SortOrder.Ascending;

        PlaylistSortOrderSymbol = PlaylistSortOrder == SortOrder.Ascending
                                ? "↑"
                                : "↓";
    }

    [RelayCommand]
    private async Task SearchPlaylists(string? searchQuery)
    {
        IEnumerable<PlaylistSummaryModel> results;
        var playlistType = MapManagerTypeToPlaylistType(_selectedManagerType);

        if (string.IsNullOrEmpty(searchQuery))
        {
            results = await _playlistFacade.GetPlaylistsByTypeAsync(playlistType);
        }
        else
        {
            results = await _playlistFacade.GetPlaylistsByNameAsync(searchQuery, playlistType);
        }

        Playlists.Clear();
        foreach (var playlist in results)
        {
            Playlists.Add(playlist);
        }
    }

    [RelayCommand]
    private async Task CreatePlaylist()
    {
        var newPlaylist = PlaylistSummaryModel.Empty with
        {
            PlaylistId = Guid.NewGuid(),
            Type = MapManagerTypeToPlaylistType(_selectedManagerType),
            Title = "New playlist",
            Description = "Description of the new playlist",
            MediaCount = 0,
            TotalDuration = 0

        };

        var savedPlaylist = await _playlistFacade.SaveAsync(newPlaylist);

        MessengerService.Send(new PlaylistAddMessage(savedPlaylist));
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
            await _navigationService.GoToAsync("/media");

            MessengerService.Send(new PlaylistDisplayMessage(playlist.PlaylistId, _selectedManagerType));
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

        var savedPlaylist = await _playlistFacade.SaveAsync(updatedPlaylist);

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
    private async Task DeletePlaylist(Guid playlistId)
    {
        if (IsEditMode && CurrentlyEditedPlaylist?.PlaylistId == playlistId)
        {
            CurrentlyEditedPlaylist = null;
            EditedPlaylistTitle = string.Empty;
        }

        await _playlistFacade.DeleteAsync(playlistId);

        var playlist = Playlists.FirstOrDefault(p => p.PlaylistId == playlistId);
        if (playlist != null)
        {
            Playlists.Remove(playlist);
        }

        MessengerService.Send(new PlaylistDeleteMessage(playlistId.ToString()));
    }

    [RelayCommand]
    private async Task GoBack()
    {
        if (IsEditMode && CurrentlyEditedPlaylist != null)
        {
            await FinishCurrentEditingAsync();
        }

        await _navigationService.GoToAsync("//select");
    }

    [RelayCommand]
    private async Task NavigateToSettings()
    {
        await _navigationService.GoToAsync("/settings");
    }

    private PlaylistType MapManagerTypeToPlaylistType(ManagerType managerType)
    {
        return managerType switch
        {
            ManagerType.Video     => PlaylistType.Video,
            ManagerType.Music     => PlaylistType.Music,
            ManagerType.AudioBook => PlaylistType.AudioBook,
            _                     => PlaylistType.Music
        };
    }

    [RelayCommand]
    private async Task SavePlaylist(PlaylistSummaryModel updatedPlaylist)
    {
        if (updatedPlaylist == null)
        {
            return;
        }

        await _playlistFacade.SaveAsync(updatedPlaylist);
    }
}
