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
public partial class PlaylistOverviewViewModel : ViewModelBase,
                                                 IRecipient<PlaylistAddMessage>,
                                                 IRecipient<PlaylistDeleteMessage>,
                                                 IRecipient<PlaylistEditMessage>
{
    private readonly IPlaylistFacade _playlistFacade;
    private readonly INavigationService _navigationService;

    public ObservableCollection<PlaylistSummaryModel> Playlists { get; set; } = new();
    public string PlaylistSearchQuery { get; set; } = string.Empty;
    public string MediumSearchQuery { get; set; } = string.Empty;
    public string SelectedSortOption { get; set; } = string.Empty;

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
        var playlists = await _playlistFacade.GetAsyncSummary();

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
            "Název"       => Playlists.OrderBy(p => p.Title),
            "Počet médií" => Playlists.OrderByDescending(p => p.MediaCount),
            "Délka"       => Playlists.OrderByDescending(p => p.TotalDuration),
            _             => Playlists
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
        if (string.IsNullOrEmpty(searchQuery))
        {
            await LoadPlaylists();
            return;
        }

        var filteredPlaylists = Playlists.Where(p =>
                                                    p.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                                                    (p.Description != null && p.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)))
                                         .ToList();

        Playlists.Clear();
        foreach (var playlist in filteredPlaylists)
        {
            Playlists.Add(playlist);
        }
    }

    [RelayCommand]
    private async Task SearchMedia(string? searchQuery)
    {
        if (string.IsNullOrEmpty(searchQuery))
        {
            await LoadPlaylists();
            return;
        }

        var filteredPlaylists = Playlists.Where(p =>
                                                    p.Medias.Any(m => m.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)))
                                         .ToList();

        Playlists.Clear();
        foreach (var playlist in filteredPlaylists)
        {
            Playlists.Add(playlist);
        }
    }

    [RelayCommand]
    private void CreatePlaylist()
    {
        var newPlaylist = PlaylistSummaryModel.Empty with
        {
            PlaylistId = Guid.NewGuid(),
            Title = "Nový playlist",
            Description = "Popis nového playlistu",
            MediaCount = 0,
            TotalDuration = 0
        };

        MessengerService.Send(new PlaylistAddMessage(newPlaylist));

        _navigationService.GoToAsync($"playlist?id={newPlaylist.PlaylistId}");
    }

    [RelayCommand]
    private async Task DeletePlaylist(PlaylistSummaryModel? playlist)
    {
        if (playlist == null) return;

        await _playlistFacade.DeleteAsync(playlist.PlaylistId);
        MessengerService.Send(new PlaylistDeleteMessage(playlist.PlaylistId.ToString()));
    }

    [RelayCommand]
    private void SelectPlaylist(PlaylistSummaryModel? playlist)
    {
        if (playlist == null) return;

        MessengerService.Send(new PlaylistSelectedMessage(playlist));

        _navigationService.GoToAsync($"playlist?id={playlist.PlaylistId}");
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.GoToAsync("//select/manager");
    }
}
