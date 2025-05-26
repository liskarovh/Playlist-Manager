using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PlaylistManager.App.Messages;
using PlaylistManager.App.Services;
using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Models;
using PlaylistManager.BL.Enums;
using PropertyChanged;

namespace PlaylistManager.App.ViewModels;

[AddINotifyPropertyChangedInterface]
public partial class PlaylistSelectedViewModel : PlaylistBaseViewModel,
                                                 IRecipient<MediumAddedMessage>,
                                                 IRecipient<MediumRemovedMessage>,
                                                 IRecipient<MediumEditedMessage>,
                                                 IRecipient<PlaylistDisplayMessage>
{
    private readonly MediumOperationsHandler _mediumHandler;
    private Guid _playlistId;
    private bool _isPlaylistSelected;

    public PlaylistSummaryModel? Playlist { get; set; }
    public string PlaylistTitle { get; set; } = string.Empty;
    public string? PlaylistDescription { get; set; } = string.Empty;

    public ObservableCollection<MediumSummaryModel> Media => _mediumHandler.Media;
    public int MediaCount => Media.Count;
    public double? TotalDuration => Media.Sum(m => m.Duration);
    public string MediaSearchQuery
    {
        get => _mediumHandler.MediaSearchQuery;
        set => _mediumHandler.MediaSearchQuery = value;
    }
    public string AuthorFilterQuery
    {
        get => _mediumHandler.AuthorFilterQuery;
        set => _mediumHandler.AuthorFilterQuery = value;
    }
    public string SelectedMediaSortOption
    {
        get => _mediumHandler.SelectedMediaSortOption;
        set => _mediumHandler.SelectedMediaSortOption = value;
    }
    public SortOrder MediaSortOrder
    {
        get => _mediumHandler.MediaSortOrder;
        set => _mediumHandler.MediaSortOrder = value;
    }
    public string MediaSortOrderSymbol
    {
        get => _mediumHandler.MediaSortOrderSymbol;
        set => _mediumHandler.MediaSortOrderSymbol = value;
    }
    public ObservableCollection<string> MediaSortOptions => _mediumHandler.MediaSortOptions;

    public PlaylistSelectedViewModel(
        IPlaylistFacade playlistFacade,
        IMediumFacade mediumFacade,
        INavigationService navigationService,
        IMessengerService messengerService)
        : base(playlistFacade, navigationService, messengerService)
    {
        _mediumHandler = new MediumOperationsHandler(playlistFacade, mediumFacade, messengerService);
        _isPlaylistSelected = false;

        PropertyChanged += async (_, args) =>
                           {
                               switch (args.PropertyName)
                               {
                                   case nameof(MediaSearchQuery):
                                       await _mediumHandler.SearchMediaAsync(MediaSearchQuery);
                                       break;
                                   case nameof(AuthorFilterQuery):
                                       await _mediumHandler.FilterMediaByAuthorAsync(AuthorFilterQuery);
                                       break;
                                   case nameof(SelectedMediaSortOption):
                                   case nameof(MediaSortOrder):
                                       await _mediumHandler.SortMediaAsync(SelectedMediaSortOption);
                                       break;
                               }
                           };

        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public bool IsPlaylistSelected
    {
        get => _isPlaylistSelected;
        set => SetProperty(ref _isPlaylistSelected, value);
    }

    protected override async Task LoadDataAsync()
    {
        await base.LoadDataAsync();

        if (_playlistId != Guid.Empty)
        {
            await LoadPlaylistAndRelatedData(_playlistId);
        }
    }

    private async Task LoadPlaylistSummary()
    {
        var playlist = await PlaylistFacade.GetPlaylistByIdAsync(_playlistId);
        if (playlist != null)
        {
            Playlist = playlist;
            PlaylistTitle = playlist.Title;
            PlaylistDescription = playlist.Description;
        }
    }

    private async Task LoadPlaylistAndRelatedData(Guid playlistId)
    {
        _playlistId = playlistId;
        _mediumHandler.SetCurrentPlaylistId(playlistId);

        await LoadPlaylistSummary();
        await _mediumHandler.LoadMediaAsync();

        IsPlaylistSelected = Playlist != null;
    }

    [RelayCommand]
    private async Task SortMedia(string? sortOption)
    {
        await _mediumHandler.SortMediaAsync(sortOption);
    }

    [RelayCommand]
    private async Task ToggleMediaSortOrder()
    {
        _mediumHandler.ToggleMediaSortOrder();

        OnPropertyChanged(nameof(MediaSortOrder));
        OnPropertyChanged(nameof(MediaSortOrderSymbol));

        await _mediumHandler.SortMediaAsync(SelectedMediaSortOption);
    }

    [RelayCommand]
    private async Task SearchMedia(string? searchQuery)
    {
        await _mediumHandler.SearchMediaAsync(searchQuery);
    }

    [RelayCommand]
    private async Task FilterMediaByAuthor(string? filterQuery)
    {
        await _mediumHandler.FilterMediaByAuthorAsync(filterQuery);
    }

    [RelayCommand]
    private async Task DeleteMedium(MediumSummaryModel medium)
    {
        await _mediumHandler.DeleteMediumAsync(medium);
    }

    [RelayCommand]
    private async Task SelectMedium(MediumSummaryModel? medium)
    {
        if (medium == null) return;

        await NavigationService.GoToAsync("//mediumDetail");

        MessengerService.Send(new ManagerSelectedMessage
        {
            SelectedType = SelectedManagerType
        });

        MessengerService.Send(new MediumSelectedMessage(_playlistId, medium.MediumId, SelectedManagerType));
    }

    [RelayCommand]
    private async Task SelectPlaylist(PlaylistSummaryModel? playlist)
    {
        if (playlist == null) return;

        await LoadPlaylistAndRelatedData(playlist.PlaylistId);
        MessengerService.Send(new PlaylistSelectedMessage(playlist));
    }

    [RelayCommand]
    private async Task CreateMedium()
    {
        await _mediumHandler.CreateMediumAsync();
    }

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

        if (playlistId == _playlistId)
        {
            IsPlaylistSelected = false;
            await GoBack();
        }
    }

    [RelayCommand]
    private void TogglePlaylistSortOrder()
    {
        TogglePlaylistSortOrderInternal();
    }

    [RelayCommand]
    private async Task SearchPlaylists(string? searchQuery)
    {
        await SearchPlaylistsAsync(searchQuery);
    }

    [RelayCommand]
    private async Task SortPlaylists(string? sortOption)
    {
        await SortPlaylistsAsync(sortOption);
    }

    [RelayCommand]
    private void ClearPlaylistSelection()
    {
        IsPlaylistSelected = false;
        _playlistId = Guid.Empty;

        Playlist = null;
        Media.Clear();
        PlaylistTitle = string.Empty;
        PlaylistDescription = string.Empty;
    }

    public void Receive(MediumAddedMessage message)
    {
        _mediumHandler.HandleMediumAddedMessage(message);
    }

    public void Receive(MediumRemovedMessage message)
    {
        _mediumHandler.HandleMediumRemovedMessage(message);
    }

    public void Receive(MediumEditedMessage message)
    {
        _mediumHandler.HandleMediumEditedMessage(message);
    }

    public async void Receive(PlaylistDisplayMessage message)
    {
        SelectedManagerType = message.ManagerType;
        await LoadPlaylistAndRelatedData(message.SelectedPlaylistId);
        await LoadPlaylistsAsync();
    }

    [RelayCommand]
    private async Task GoBack()
    {
        IsPlaylistSelected = false;
        await NavigationService.GoToAsync("/playlists");
    }

    [RelayCommand]
    private async Task GoToSelect()
    {
        await NavigationService.GoToAsync("//select");
    }

    [RelayCommand]
    private async Task NavigateToSettings()
    {
        await NavigationService.GoToAsync("/settings");
    }

}
