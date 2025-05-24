using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PlaylistManager.App.Messages;
using PlaylistManager.App.Services;
using PlaylistManager.App.Views;
using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Models;
using PlaylistManager.Common.Enums;
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
    public string MediumSearchQuery { get; set; } = string.Empty;
    public string SelectedSortOption { get; set; } = "Name";

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
                                   case nameof(MediumSearchQuery):
                                       await SearchMediaCommand.ExecuteAsync(MediumSearchQuery);
                                       break;
                                   case nameof(SelectedSortOption):
                                       SortPlaylistsCommand.Execute(SelectedSortOption);
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
    }

    [RelayCommand]
    private void SortPlaylists(string? sortOption)
    {
        if (string.IsNullOrEmpty(sortOption)) return;

        IEnumerable<PlaylistSummaryModel> sortedPlaylists = sortOption switch
        {
            "Name"           => Playlists.OrderBy(p => p.Title),
            "Media Count"    => Playlists.OrderByDescending(p => p.MediaCount),
            "Total Duration" => Playlists.OrderByDescending(p => p.TotalDuration),
            _                => Playlists
        };

        var newList = sortedPlaylists.ToList();
        Playlists.Clear();
        foreach (var item in newList)
        {
            Playlists.Add(item);
        }
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
            //TODO: results = await _playlistFacade.GetPlaylistsByNameAsync(searchQuery);
            results = await _playlistFacade.GetAsyncSummary(); // Placeholder
        }

        Playlists.Clear();
        foreach (var playlist in results)
        {
            Playlists.Add(playlist);
        }
    }

    [RelayCommand]
    private async Task SearchMedia(string? searchQuery)
    {
        IEnumerable<PlaylistSummaryModel> results;

        if (string.IsNullOrEmpty(searchQuery))
        {
            results = await _playlistFacade.GetAsyncSummary();
        }
        else
        {
           //TODO: results = await _playlistFacade.GetPlaylistsByMediaTitleAsync(searchQuery);
           results = await _playlistFacade.GetAsyncSummary(); // Placeholder
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

        //TODO: await _navigationService.GoToAsync($"playlist?id={savedPlaylist.PlaylistId}");
    }

    [RelayCommand]
    private void SelectPlaylist(PlaylistSummaryModel? playlist)
    {
        if (playlist == null) return;

        MessengerService.Send(new PlaylistSelectedMessage(playlist));

        //TODO: _navigationService.GoToAsync($"playlist?id={playlist.PlaylistId}");
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
            _                     => PlaylistType.Music // Výchozí hodnota pro NotDecided
        };
    }
}
