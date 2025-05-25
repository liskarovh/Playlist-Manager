using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PlaylistManager.App.Messages;
using PlaylistManager.App.Services;
using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Models;
using PropertyChanged;

namespace PlaylistManager.App.ViewModels;

[AddINotifyPropertyChangedInterface]
public partial class PlaylistSelectedViewModel : ViewModelBase,
                                               IRecipient<MediumAddedMessage>,
                                               IRecipient<MediumRemovedMessage>,
                                               IRecipient<MediumEditedMessage>
{
    private readonly IPlaylistFacade _playlistFacade;
    private readonly IMediumFacade _mediumFacade;
    private readonly INavigationService _navigationService;
    private Guid _playlistId;

    public PlaylistSummaryModel? Playlist { get; set; }
    public ObservableCollection<MediumSummaryModel> Media { get; set; } = new();
    public string PlaylistTitle { get; set; } = string.Empty;
    public string PlaylistDescription { get; set; } = string.Empty;
    public int MediaCount => Media.Count;
    public double? TotalDuration => Media.Sum(m => m.Duration);

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
    }

    protected override async Task LoadDataAsync()
    {
        if (_playlistId != Guid.Empty)
        {
            await LoadPlaylistAsync();
            await LoadMediaAsync();
        }
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
    private async Task AddNewMedium()
    {
        // TODO: add navigation logic
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
        }
    }
}
