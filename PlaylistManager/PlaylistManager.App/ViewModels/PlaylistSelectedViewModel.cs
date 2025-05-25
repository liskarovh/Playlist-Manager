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
public partial class PlaylistSelectedViewModel : ViewModelBase,
                                               IRecipient<MediumAddedMessage>,
                                               IRecipient<MediumRemovedMessage>,
                                               IRecipient<MediumEditedMessage>,
                                               IRecipient<PlaylistAddMessage>,
                                               IRecipient<PlaylistDeleteMessage>,
                                               IRecipient<PlaylistEditMessage>
{
    private readonly IPlaylistFacade _playlistFacade;
    private readonly IMediumFacade _mediumFacade;
    private readonly INavigationService _navigationService;
    private Guid _playlistId;
    private ManagerType _selectedManagerType = ManagerType.NotDecided;

    public PlaylistSummaryModel? Playlist { get; set; }
    public ObservableCollection<MediumSummaryModel> Media { get; set; } = new();
    public ObservableCollection<PlaylistSummaryModel> Playlists { get; set; } = new();
    public string PlaylistTitle { get; set; } = string.Empty;
    public string PlaylistDescription { get; set; } = string.Empty;
    public int MediaCount => Media.Count;
    public double? TotalDuration => Media.Sum(m => m.Duration);

    public string PlaylistSearchQuery { get; set; } = string.Empty;
    public string MediaSearchQuery { get; set; } = string.Empty;

    public string SelectedPlaylistSortOption { get; set; } = "Name";
    public string SelectedMediaSortOption { get; set; } = "Name";
    public SortOrder PlaylistSortOrder { get; set; } = SortOrder.Ascending;
    public SortOrder MediaSortOrder { get; set; } = SortOrder.Ascending;
    public ObservableCollection<string> PlaylistSortOptions { get; } = new(["Name", "Media Count", "Total Duration"]);
    public ObservableCollection<string> MediaSortOptions { get; } = new(["Title", "Author", "Added Date", "Duration"]);

    public PlaylistSelectedViewModel(
        IPlaylistFacade playlistFacade,
        IMediumFacade mediumFacade,
        INavigationService navigationService,
        IMessengerService messengerService)
        : base(messengerService)
    {
        _playlistFacade = playlistFacade;
        _mediumFacade = mediumFacade;
        _navigationService = navigationService;

        PropertyChanged += async (_, args) =>
                           {
                               switch (args.PropertyName)
                               {
                                   case nameof(PlaylistSearchQuery):
                                       await SearchPlaylistsCommand.ExecuteAsync(PlaylistSearchQuery);
                                       break;
                                   case nameof(MediaSearchQuery):
                                       await SearchMediaCommand.ExecuteAsync(MediaSearchQuery);
                                       break;
                                   case nameof(SelectedPlaylistSortOption):
                                   case nameof(PlaylistSortOrder):
                                       await SortPlaylistsCommand.ExecuteAsync(SelectedPlaylistSortOption);
                                       break;
                                   case nameof(SelectedMediaSortOption):
                                   case nameof(MediaSortOrder):
                                       await SortMediaCommand.ExecuteAsync(SelectedMediaSortOption);
                                       break;
                               }
                           };

        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("playlistId", out var playlistIdObj) &&
            playlistIdObj is Guid playlistId)
        {
            _playlistId = playlistId;
            LoadPlaylistData();
        }
    }

    private async void LoadPlaylistData()
    {
        await LoadPlaylistAsync();
        await LoadMediaAsync();
        await LoadPlaylistsAsync();
    }

    protected override async Task LoadDataAsync()
    {
        if (_playlistId != Guid.Empty)
        {
            await LoadPlaylistAsync();
            await LoadMediaAsync();
        }

        await LoadPlaylistsAsync();
    }

    private async Task LoadPlaylistAsync()
    {
        var playlist = await _playlistFacade.GetPlaylistByIdAsync(_playlistId);
        if (playlist != null)
        {
            Playlist = playlist;
            PlaylistTitle = playlist.Title;
            PlaylistDescription = playlist.Description;
        }
    }

    private async Task LoadMediaAsync()
    {
        if (_playlistId != Guid.Empty)
        {
            var media = await _mediumFacade.GetMediaByPlaylistIdAsync(_playlistId);
            Media.Clear();
            foreach (var medium in media)
            {
                Media.Add(medium);
            }

            await SortMediaCommand.ExecuteAsync(SelectedMediaSortOption);
        }
    }

    private async Task LoadPlaylistsAsync()
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
    private async Task SortMedia(string? sortOption)
    {
        if (string.IsNullOrEmpty(sortOption) || _playlistId == Guid.Empty) return;

        MediaSortBy sortBy = sortOption switch
        {
            "Title"          => MediaSortBy.Title,
            "Author"         => MediaSortBy.Author,
            "Added Date"     => MediaSortBy.AddedDate,
            "Duration"       => MediaSortBy.Duration,
            _                => MediaSortBy.Title
        };

        var sortedMedia = await _playlistFacade.GetMediaInPlaylistSortedAsync(
                                                                              _playlistId,
                                                                              null,
                                                                              sortBy,
                                                                              MediaSortOrder);

        Media.Clear();
        foreach (var medium in sortedMedia)
        {
            Media.Add(medium);
        }
    }

    [RelayCommand]
    private void TogglePlaylistSortOrder()
    {
        PlaylistSortOrder = PlaylistSortOrder == SortOrder.Ascending
                                ? SortOrder.Descending
                                : SortOrder.Ascending;
    }

    [RelayCommand]
    private void ToggleMediaSortOrder()
    {
        MediaSortOrder = MediaSortOrder == SortOrder.Ascending
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
    private async Task SearchMedia(string? searchQuery)
    {
        if (_playlistId == Guid.Empty) return;

        IEnumerable<MediumSummaryModel> results;

        if (string.IsNullOrEmpty(searchQuery))
        {
            results = await _mediumFacade.GetMediaByPlaylistIdAsync(_playlistId);
        }
        else
        {
            results = await _playlistFacade.GetMediaInPlaylistByTitleAsync(_playlistId, searchQuery);
        }

        Media.Clear();
        foreach (var medium in results)
        {
            Media.Add(medium);
        }
    }

    [RelayCommand]
    private async Task DeleteMedium(MediumSummaryModel medium)
    {
        if (medium == null) return;

        await _mediumFacade.DeleteAsync(medium.Id);
        Media.Remove(medium);

        MessengerService.Send(new MediumRemovedMessage(medium.Id));
    }

    [RelayCommand]
    private async Task SelectMedium(MediumSummaryModel medium)
    {
        if (medium == null) return;

        MessengerService.Send(new MediumSelectedMessage(medium));

        var parameters = new Dictionary<string, object?>
        {
            { "mediumId", medium.Id },
            { "playlistId", _playlistId }
        };

        await _navigationService.GoToAsync<MediumSelectedViewModel>(parameters);
    }

    [RelayCommand]
    private async Task SelectPlaylist(PlaylistSummaryModel? playlist)
    {
        if (playlist == null) return;

        _playlistId = playlist.PlaylistId;
        await LoadPlaylistAsync();
        await LoadMediaAsync();

        MessengerService.Send(new PlaylistSelectedMessage(playlist));
    }

    [RelayCommand]
    private async Task CreateMedium()
    {
        if (_playlistId == Guid.Empty) return;

        var mediumId = Guid.NewGuid();

        var newMedium = MediumDetailedModel.Empty with
        {
            Id = Guid.NewGuid(),
            MediumId = mediumId,
            PlaylistId = _playlistId,
            Title = "New Medium",
            Description = "Description of the new medium"
        };

        await _mediumFacade.SaveAsync(newMedium);

        var media = await _mediumFacade.GetMediaByPlaylistIdAsync(_playlistId);
        var summaryMedium = media.FirstOrDefault(m => m.MediumId == mediumId);

        if (summaryMedium != null)
        {
            Media.Add(summaryMedium);
            MessengerService.Send(new MediumAddedMessage(summaryMedium, _playlistId));
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
    private async Task DeletePlaylist(Guid playlistId)
    {
        await _playlistFacade.DeleteAsync(playlistId);

        var playlist = Playlists.FirstOrDefault(p => p.PlaylistId == playlistId);
        if (playlist != null)
        {
            Playlists.Remove(playlist);
        }

        if (playlistId == _playlistId)
        {
            await GoBack();
        }

        MessengerService.Send(new PlaylistDeleteMessage(playlistId.ToString()));
    }

    [RelayCommand]
    private async Task GoBack()
    {
        await _navigationService.GoToAsync("//playlists");
    }

    public void Receive(MediumAddedMessage message)
    {
        if (message.PlaylistId == _playlistId)
        {
            Media.Add(message.Medium);
            SortMediaCommand.ExecuteAsync(SelectedMediaSortOption);
        }
    }

    public void Receive(MediumRemovedMessage message)
    {
        var medium = Media.FirstOrDefault(m => m.Id == message.MediumId);
        if (medium != null)
        {
            Media.Remove(medium);
        }
    }

    public void Receive(MediumEditedMessage message)
    {
        var index = Media.ToList().FindIndex(m => m.Id == message.Medium.Id);
        if (index >= 0)
        {
            Media[index] = message.Medium;
            SortMediaCommand.ExecuteAsync(SelectedMediaSortOption);
        }
    }

    public void Receive(PlaylistAddMessage message)
    {
        var playlistType = MapManagerTypeToPlaylistType(_selectedManagerType);

        if (message.Value.Type == playlistType)
        {
            Playlists.Add(message.Value);
            SortPlaylistsCommand.ExecuteAsync(SelectedPlaylistSortOption);
        }
    }

    public void Receive(PlaylistDeleteMessage message)
    {
        var playlist = Playlists.FirstOrDefault(p => p.PlaylistId.ToString() == message.Value);
        if (playlist != null)
        {
            Playlists.Remove(playlist);
        }
    }

    public void Receive(PlaylistEditMessage message)
    {
        var index = Playlists.ToList().FindIndex(p => p.PlaylistId == message.Value.PlaylistId);
        if (index >= 0)
        {
            Playlists[index] = message.Value;

            if (message.Value.PlaylistId == _playlistId)
            {
                Playlist = message.Value;
                PlaylistTitle = message.Value.Title;
                PlaylistDescription = message.Value.Description;
            }

            SortPlaylistsCommand.ExecuteAsync(SelectedPlaylistSortOption);
        }
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
