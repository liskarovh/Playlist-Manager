using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PlaylistManager.App.Messages;
using PlaylistManager.App.Services;
using PlaylistManager.App.Views;
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

    public string SelectedSortOption { get; set; } = "Name";
    public SortOrder CurrentSortOrder { get; set; } = SortOrder.Ascending;
    public ObservableCollection<string> SortOptions { get; } = new(["Name", "Media Count", "Total Duration"]);

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
                                   case nameof(SelectedSortOption):
                                   case nameof(CurrentSortOrder):
                                       await SortPlaylistsCommand.ExecuteAsync(SelectedSortOption);
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

        await SortPlaylistsCommand.ExecuteAsync(SelectedSortOption);
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

        var sortedPlaylists = await _playlistFacade.GetPlaylistsSortedAsync(sortBy, CurrentSortOrder, playlistType);

        Playlists.Clear();
        foreach (var item in sortedPlaylists)
        {
            Playlists.Add(item);
        }
    }

    [RelayCommand]
    private void ToggleSortOrder()
    {
        // Přepnutí směru řazení
        CurrentSortOrder = CurrentSortOrder == SortOrder.Ascending
                               ? SortOrder.Descending
                               : SortOrder.Ascending;
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

        MessengerService.Send(new PlaylistSelectedMessage(playlist));

        await _navigationService.GoToAsync("//playlist");

    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.GoToAsync("//select");
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
}
