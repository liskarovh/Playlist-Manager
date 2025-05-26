using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Messaging;
using PlaylistManager.App.Mappers;
using PlaylistManager.App.Messages;
using PlaylistManager.App.Services;
using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Models;
using PlaylistManager.BL.Enums;
using PlaylistManager.Common.Enums;

namespace PlaylistManager.App.ViewModels;

public abstract class PlaylistBaseViewModel : ViewModelBase,
                                            IRecipient<PlaylistAddMessage>,
                                            IRecipient<PlaylistDeleteMessage>,
                                            IRecipient<PlaylistEditMessage>,
                                            IRecipient<ManagerSelectedMessage>
{
    protected readonly IPlaylistFacade PlaylistFacade;
    protected readonly INavigationService NavigationService;

    protected ManagerType SelectedManagerType = ManagerType.NotDecided;

    public ObservableCollection<PlaylistSummaryModel> Playlists { get; set; } = new();
    public string PlaylistSearchQuery { get; set; } = string.Empty;

    public string SelectedPlaylistSortOption { get; set; } = "Name";
    public SortOrder PlaylistSortOrder { get; set; } = SortOrder.Ascending;
    public string PlaylistSortOrderSymbol { get; set; } = "↑";
    public ObservableCollection<string> PlaylistSortOptions { get; } = new(["Name", "Media Count", "Total Duration"]);

    protected PlaylistBaseViewModel(IPlaylistFacade playlistFacade,
                                  INavigationService navigationService,
                                  IMessengerService messengerService)
        : base(messengerService)
    {
        PlaylistFacade = playlistFacade;
        NavigationService = navigationService;

        PropertyChanged += async (_, args) =>
        {
            switch (args.PropertyName)
            {
                case nameof(PlaylistSearchQuery):
                    await SearchPlaylistsAsync(PlaylistSearchQuery);
                    break;
                case nameof(SelectedPlaylistSortOption):
                case nameof(PlaylistSortOrder):
                    await SortPlaylistsAsync(SelectedPlaylistSortOption);
                    break;
            }
        };
    }

    protected override async Task LoadDataAsync()
    {
        await LoadPlaylistsAsync();
    }

    public async void Receive(PlaylistAddMessage message)
    {
        Playlists.Add(message.Value);
        await SortPlaylistsAsync(SelectedPlaylistSortOption);
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
            Playlists[index] = message.Value;
    }

    public async void Receive(ManagerSelectedMessage message)
    {
        SelectedManagerType = message.SelectedType;
        await LoadDataAsync();
    }

    protected async Task LoadPlaylistsAsync()
    {
        var playlistType = PlaylistTypeMapper.MapManagerTypeToPlaylistType(SelectedManagerType);
        var playlists = await PlaylistFacade.GetPlaylistsByTypeAsync(playlistType);

        Playlists.Clear();
        foreach (var playlist in playlists)
        {
            Playlists.Add(playlist);
        }

        await SortPlaylistsAsync(SelectedPlaylistSortOption);
    }

    protected async Task SortPlaylistsAsync(string? sortOption)
    {
        if (string.IsNullOrEmpty(sortOption)) return;

        PlaylistSortBy sortBy = sortOption switch
        {
            "Name" => PlaylistSortBy.Title,
            "Media Count" => PlaylistSortBy.MediaCount,
            "Total Duration" => PlaylistSortBy.TotalDuration,
            _ => PlaylistSortBy.Title
        };

        var playlistType = PlaylistTypeMapper.MapManagerTypeToPlaylistType(SelectedManagerType);
        var sortedPlaylists = await PlaylistFacade.GetPlaylistsSortedAsync(sortBy, PlaylistSortOrder, playlistType);

        Playlists.Clear();
        foreach (var item in sortedPlaylists)
        {
            Playlists.Add(item);
        }
    }

    protected void TogglePlaylistSortOrderInternal()
    {
        PlaylistSortOrder = PlaylistSortOrder == SortOrder.Ascending
                             ? SortOrder.Descending
                             : SortOrder.Ascending;

        PlaylistSortOrderSymbol = PlaylistSortOrder == SortOrder.Ascending
                                ? "↑"
                                : "↓";
    }

    protected async Task SearchPlaylistsAsync(string? searchQuery)
    {
        IEnumerable<PlaylistSummaryModel> results;
        var playlistType = PlaylistTypeMapper.MapManagerTypeToPlaylistType(SelectedManagerType);

        if (string.IsNullOrEmpty(searchQuery))
        {
            results = await PlaylistFacade.GetPlaylistsByTypeAsync(playlistType);
        }
        else
        {
            results = await PlaylistFacade.GetPlaylistsByNameAsync(searchQuery, playlistType);
        }

        Playlists.Clear();
        foreach (var playlist in results)
        {
            Playlists.Add(playlist);
        }
    }

    protected async Task<PlaylistSummaryModel> CreatePlaylistInternal()
    {
        var newPlaylist = PlaylistSummaryModel.Empty with
        {
            PlaylistId = Guid.NewGuid(),
            Type = PlaylistTypeMapper.MapManagerTypeToPlaylistType(SelectedManagerType),
            Title = "New playlist",
            Description = "Description of the new playlist",
            MediaCount = 0,
            TotalDuration = 0
        };

        var savedPlaylist = await PlaylistFacade.SaveAsync(newPlaylist);
        return savedPlaylist;
    }

    protected async Task DeletePlaylistInternal(Guid playlistId)
    {
        await PlaylistFacade.DeleteAsync(playlistId);

        var playlist = Playlists.FirstOrDefault(p => p.PlaylistId == playlistId);
        if (playlist != null)
        {
            Playlists.Remove(playlist);
        }

        MessengerService.Send(new PlaylistDeleteMessage(playlistId.ToString()));
    }
}
