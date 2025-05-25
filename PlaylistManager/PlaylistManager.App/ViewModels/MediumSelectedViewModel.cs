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
public partial class MediumSelectedViewModel : ViewModelBase,
                                               IRecipient<ManagerSelectedMessage>,
                                               IRecipient<MediumSelectedMessage>,
                                               IRecipient<MediumEditedMessage>,
                                               IRecipient<MediumRemovedMessage>
{
    private readonly IMediumFacade _mediumFacade;
    private readonly INavigationService _navigationService;
    private readonly IPlaylistFacade _playlistFacade;

    private Guid _playlistId;
    private Guid _mediumId;
    private ManagerType _selectedManagerType = ManagerType.NotDecided;
    private bool _isEditMode;
    private bool _isPlaylistSelected;

    private string _originalName = string.Empty;
    private string _originalAuthor = string.Empty;
    private string _originalDescription = string.Empty;
    private string _originalURL = string.Empty;
    private string _originalDuration = string.Empty;
    private string _originalReleaseYear = string.Empty;
    private string _originalFormat = string.Empty;
    private string _originalGenre = string.Empty;

    public PlaylistSummaryModel? Playlist { get; set; }
    public ObservableCollection<MediumSummaryModel> Media { get; set; } = new();
    public ObservableCollection<PlaylistSummaryModel> Playlists { get; set; } = new();
    public string PlaylistTitle { get; set; } = string.Empty;
    public string PlaylistDescription { get; set; } = string.Empty;
    public int MediaCount => Media.Count;
    public double? TotalDuration => Media.Sum(m => m.Duration);

    public bool IsMediumSelected { get; set; }
    public string MediumName { get; set; } = string.Empty;
    public string MediumAuthor { get; set; } = string.Empty;
    public string MediumDescription { get; set; } = string.Empty;
    public string MediumURL { get; set; } = string.Empty;
    public string MediumDuration { get; set; } = string.Empty;
    public string MediumReleaseYear { get; set; } = string.Empty;
    public string MediumFormat { get; set; } = string.Empty;
    public string MediumGenre { get; set; } = string.Empty;

    public string PlaylistSearchQuery { get; set; } = string.Empty;
    public string MediaSearchQuery { get; set; } = string.Empty;
    public string AuthorFilterQuery { get; set; } = string.Empty;

    public string SelectedPlaylistSortOption { get; set; } = "Name";
    public string SelectedMediaSortOption { get; set; } = "Title";
    public SortOrder PlaylistSortOrder { get; set; } = SortOrder.Ascending;
    public SortOrder MediaSortOrder { get; set; } = SortOrder.Ascending;
    public string PlaylistSortOrderSymbol { get; set; } = "↑";
    public string MediaSortOrderSymbol { get; set; } = "↑";
    public ObservableCollection<string> PlaylistSortOptions { get; } = new(["Name", "Media Count", "Total Duration"]);

    public ObservableCollection<string> MediaSortOptions { get; } = new([
                                                                            "Title", "Author", "Added Date",
                                                                            "Duration"
                                                                        ]
                                                                       );

    public bool HasValidationErrors { get; set; }
    public string ValidationMessage { get; set; } = string.Empty;
    public bool IsSaving { get; set; }

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public bool IsNotEditMode => !IsEditMode;

    public bool IsPlaylistSelected
    {
        get => _isPlaylistSelected;
        set => SetProperty(ref _isPlaylistSelected, value);
    }

    public ObservableCollection<string> FormatOptions
    {
        get
        {
            return _selectedManagerType switch
            {
                ManagerType.Video => new(new[]
                {
                    "Mp4",
                    "Avi",
                    "Mkv",
                    "Mov",
                    "Unknown"
                }),

                ManagerType.Music or ManagerType.AudioBook => new(new[]
                {
                    "Mp3",
                    "Wav",
                    "Flac",
                    "Aac",
                    "Unknown"
                }),

                _ => new(["Unknown"])
            };
        }
    }

    public ObservableCollection<string> GenreOptions
    {
        get
        {
            return _selectedManagerType switch
            {
                ManagerType.Video => new(new[]
                {
                    "Comedy",
                    "Horror",
                    "Thriller",
                    "Fantasy",
                    "SciFi",
                    "Drama",
                    "Other",
                    "Unknown"
                }),

                ManagerType.Music => new(new[]
                {
                    "Jazz",
                    "Rock",
                    "HipHop",
                    "Classical",
                    "Electronic",
                    "Pop",
                    "Punk",
                    "Anime",
                    "Other",
                    "Unknown"
                }),

                ManagerType.AudioBook => new(new[]
                {
                    "Fiction",
                    "NonFiction",
                    "Mystery",
                    "Fantasy",
                    "SciFi",
                    "Romance",
                    "Dystopia",
                    "Unknown"
                }),

                _ => new(["Unknown"])
            };
        }
    }

    public MediumSelectedViewModel(IPlaylistFacade playlistFacade,
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
                                   case nameof(AuthorFilterQuery):
                                       await FilterMediaByAuthorCommand.ExecuteAsync(AuthorFilterQuery);
                                       break;
                                   case nameof(SelectedPlaylistSortOption):
                                   case nameof(PlaylistSortOrder):
                                       await SortPlaylistsCommand.ExecuteAsync(SelectedPlaylistSortOption);
                                       break;
                                   case nameof(SelectedMediaSortOption):
                                   case nameof(MediaSortOrder):
                                       await SortMediaCommand.ExecuteAsync(SelectedMediaSortOption);
                                       break;
                                   case nameof(MediumName):
                                   case nameof(MediumAuthor):
                                   case nameof(MediumDescription):
                                   case nameof(MediumURL):
                                   case nameof(MediumDuration):
                                   case nameof(MediumReleaseYear):
                                   case nameof(MediumFormat):
                                   case nameof(MediumGenre):
                                       if (IsEditMode && IsMediumSelected)
                                       {
                                           ValidateMediumData();
                                       }

                                       break;
                               }
                           };

        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    [RelayCommand]
    private async Task LoadMediumDetail()
    {
        if (_playlistId == Guid.Empty || _mediumId == Guid.Empty)
            return;

        try
        {
            var medium = await _mediumFacade.GetMediumByIdAsync(_mediumId);

            if (medium != null)
            {
                MediumName = medium.Title;
                MediumAuthor = medium.Author;
                MediumDescription = medium.Description;
                MediumURL = medium.Url;
                MediumDuration = medium.Duration?.ToString() ?? string.Empty;
                MediumReleaseYear = medium.ReleaseYear?.ToString() ?? string.Empty;
                MediumFormat = medium.Format;
                MediumGenre = medium.Genre;

                IsMediumSelected = true;

                BackupOriginalValues();
            }
            else
            {
                ClearMediumSelection();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while loading medium detail: {ex.Message}");
            ClearMediumSelection();
        }
    }

    private void BackupOriginalValues()
    {
        _originalName = MediumName;
        _originalAuthor = MediumAuthor;
        _originalDescription = MediumDescription;
        _originalURL = MediumURL;
        _originalDuration = MediumDuration;
        _originalReleaseYear = MediumReleaseYear;
        _originalFormat = MediumFormat;
        _originalGenre = MediumGenre;
    }

    private void RestoreOriginalValues()
    {
        MediumName = _originalName;
        MediumAuthor = _originalAuthor;
        MediumDescription = _originalDescription;
        MediumURL = _originalURL;
        MediumDuration = _originalDuration;
        MediumReleaseYear = _originalReleaseYear;
        MediumFormat = _originalFormat;
        MediumGenre = _originalGenre;
    }

    private async Task LoadPlaylistAndRelatedData(Guid playlistId)
    {
        _playlistId = playlistId;

        await LoadPlaylistSummary();

        await LoadMedia();

        if (Playlists.Count == 0)
        {
            await LoadAllPlaylists();
        }

        IsPlaylistSelected = Playlist != null;
    }

    private async Task LoadPlaylistSummary()
    {
        var playlist = await _playlistFacade.GetPlaylistByIdAsync(_playlistId);

        if (playlist != null)
        {
            Playlist = playlist;
            PlaylistTitle = playlist.Title;
            PlaylistDescription = playlist.Description;
        }
    }

    private async Task LoadMedia()
    {
        if (_playlistId == Guid.Empty) return;

        try
        {
            var media = await _playlistFacade.GetMediaInPlaylistSortedAsync(
                                                                            _playlistId,
                                                                            null,
                                                                            null,
                                                                            MediaSortBy.Title,
                                                                            MediaSortOrder);

            // Přidání logování
            System.Diagnostics.Debug.WriteLine($"Načteno {media.Count()} médií pro playlist {_playlistId}");

            Media.Clear();
            foreach (var medium in media)
            {
                Media.Add(medium);
            }

            // Výpis aktuálního počtu médií v kolekci
            System.Diagnostics.Debug.WriteLine($"Počet médií v kolekci po přidání: {Media.Count}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Chyba při načítání médií: {ex.Message}");
        }
    }

    private async Task LoadAllPlaylists()
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

    private void ClearMediumSelection()
    {
        IsMediumSelected = false;
        IsEditMode = false;
        MediumName = string.Empty;
        MediumAuthor = string.Empty;
        MediumDescription = string.Empty;
        MediumURL = string.Empty;
        MediumDuration = string.Empty;
        MediumReleaseYear = string.Empty;
        MediumFormat = string.Empty;
        MediumGenre = string.Empty;
        ValidationMessage = string.Empty;
        HasValidationErrors = false;
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
            "Title"      => MediaSortBy.Title,
            "Author"     => MediaSortBy.Author,
            "Added Date" => MediaSortBy.AddedDate,
            "Duration"   => MediaSortBy.Duration,
            _            => MediaSortBy.Title
        };

        var sortedMedia =
            await _playlistFacade.GetMediaInPlaylistSortedAsync(
                                                                _playlistId,
                                                                null,
                                                                null,
                                                                sortBy,
                                                                MediaSortOrder
                                                               );
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

        PlaylistSortOrderSymbol = PlaylistSortOrder == SortOrder.Ascending
                                      ? "↑"
                                      : "↓";
    }

    [RelayCommand]
    private void ToggleMediaSortOrder()
    {
        MediaSortOrder = MediaSortOrder == SortOrder.Ascending
                             ? SortOrder.Descending
                             : SortOrder.Ascending;

        MediaSortOrderSymbol = MediaSortOrder == SortOrder.Ascending
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
    private async Task SearchMedia(string? searchQuery)
    {
        if (_playlistId == Guid.Empty) return;

        IEnumerable<MediumSummaryModel> results;

        if (string.IsNullOrEmpty(searchQuery))
        {
            results = await _playlistFacade.GetMediaInPlaylistSortedAsync(
                                                                          _playlistId,
                                                                          null,
                                                                          null,
                                                                          MediaSortBy.Title,
                                                                          MediaSortOrder
                                                                         );
        }
        else
        {
            results = await _playlistFacade.GetMediaInPlaylistSortedAsync(
                                                                          _playlistId,
                                                                          MediaFilterBy.Title,
                                                                          searchQuery,
                                                                          MediaSortBy.Title,
                                                                          MediaSortOrder
                                                                         );
        }

        Media.Clear();

        foreach (var medium in results)
        {
            Media.Add(medium);
        }
    }

    [RelayCommand]
    private async Task FilterMediaByAuthor(string? filterQuery)
    {
        if (_playlistId == Guid.Empty) return;

        IEnumerable<MediumSummaryModel> results;

        if (string.IsNullOrEmpty(filterQuery))
        {
            results = await _playlistFacade.GetMediaInPlaylistSortedAsync(
                                                                          _playlistId,
                                                                          null,
                                                                          null,
                                                                          MediaSortBy.Title,
                                                                          MediaSortOrder
                                                                         );
        }
        else
        {
            results = await _playlistFacade.GetMediaInPlaylistSortedAsync(
                                                                          _playlistId,
                                                                          MediaFilterBy.Author,
                                                                          filterQuery,
                                                                          MediaSortBy.Title,
                                                                          MediaSortOrder
                                                                         );
        }

        Media.Clear();

        foreach (var medium in results)
        {
            Media.Add(medium);
        }
    }

    [RelayCommand]
    private async Task SelectMedium(MediumSummaryModel? medium)
    {
        if (medium == null) return;

        if (medium.MediumId == _mediumId && IsMediumSelected)
            return;

        _mediumId = medium.MediumId;

        await LoadMediumDetailCommand.ExecuteAsync(null);

        MessengerService.Send(new MediumSelectedMessage(_playlistId, medium.MediumId, _selectedManagerType));
    }

    [RelayCommand]
    private async Task CreateMedium()
    {
        if (_playlistId == Guid.Empty) return;

        var mediumId = Guid.NewGuid();

        try
        {
            var newMedium = MediumDetailedModel.Empty with
            {
                Id = Guid.NewGuid(),
                MediumId = mediumId,
                PlaylistId = _playlistId,
                Title = "New Medium",
                Description = "Description of the new medium",
                Author = "Unknown Author",
                Duration = 0,
                AddedDate = DateTime.Now,
                Format = "Unknown",
                Genre = "Unknown"
            };

            await _mediumFacade.SaveAsync(newMedium);

            var summaryMedium = new MediumSummaryModel
            {
                Id = newMedium.Id,
                MediumId = newMedium.MediumId,
                PlaylistId = _playlistId,
                Title = newMedium.Title,
                Author = newMedium.Author,
                Duration = newMedium.Duration,
                AddedDate = newMedium.AddedDate
            };

            Media.Add(summaryMedium);

            await SelectMediumCommand.ExecuteAsync(summaryMedium);
            StartEditCommand.Execute(null);

            MessengerService.Send(new MediumAddedMessage(summaryMedium, _playlistId));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error while creating medium: {ex.Message}");
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
    private async Task SelectPlaylist(PlaylistSummaryModel? playlist)
    {
        if (playlist == null) return;

        await _navigationService.GoToAsync("//media");
        MessengerService.Send(new PlaylistDisplayMessage(playlist.PlaylistId, _selectedManagerType));
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
    private void StartEdit()
    {
        BackupOriginalValues();

        IsEditMode = true;

        ValidationMessage = string.Empty;
        HasValidationErrors = false;
    }

    [RelayCommand]
    private void CancelEdit()
    {
        RestoreOriginalValues();

        IsEditMode = false;

        ValidationMessage = string.Empty;
        HasValidationErrors = false;
    }

    private bool ValidateMediumData()
    {
        HasValidationErrors = false;
        ValidationMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(MediumName))
        {
            ValidationMessage = "Media title is required.";
            HasValidationErrors = true;
            return false;
        }

        if (!string.IsNullOrEmpty(MediumReleaseYear))
        {
            if (!int.TryParse(MediumReleaseYear, out int year) || year < 1800 || year > DateTime.Now.Year)
            {
                ValidationMessage = $"Release year msut be a value between 1800 and {DateTime.Now.Year}.";
                HasValidationErrors = true;
                return false;
            }
        }

        if (!string.IsNullOrEmpty(MediumDuration))
        {
            if (!int.TryParse(MediumDuration, out int duration) || duration < 0)
            {
                ValidationMessage = "Medium duration must be a positive number.";
                HasValidationErrors = true;
                return false;
            }
        }

        return true;
    }

    [RelayCommand]
    private async Task SaveMedium()
    {
        if (!IsMediumSelected || _mediumId == Guid.Empty)
            return;

        if (!ValidateMediumData())
        {
            return;
        }

        try
        {
            IsSaving = true;

            int? duration = null;

            if (int.TryParse(MediumDuration, out var parsedDuration))
            {
                duration = parsedDuration;
            }

            int? releaseYear = null;

            if (int.TryParse(MediumReleaseYear, out var parsedYear))
            {
                releaseYear = parsedYear;
            }

            var mediumToSave = new MediumDetailedModel
            {
                Id = Guid.NewGuid(),
                MediumId = _mediumId,
                PlaylistId = _playlistId,
                Title = MediumName,
                Author = MediumAuthor,
                Description = MediumDescription,
                Url = MediumURL,
                Duration = duration,
                ReleaseYear = releaseYear,
                Format = MediumFormat,
                Genre = MediumGenre,
                AddedDate = DateTime.Now
            };

            var savedMedium = await _mediumFacade.SaveAsync(mediumToSave);

            IsEditMode = false;

            var summaryModel = new MediumSummaryModel
            {
                Id = savedMedium.Id,
                MediumId = savedMedium.MediumId,
                PlaylistId = savedMedium.PlaylistId,
                Title = savedMedium.Title,
                Author = savedMedium.Author,
                Duration = savedMedium.Duration,
                AddedDate = savedMedium.AddedDate
            };

            var existingItem = Media.FirstOrDefault(m => m.MediumId == _mediumId);

            if (existingItem != null)
            {
                int index = Media.IndexOf(existingItem);
                Media[index] = summaryModel;
            }

            BackupOriginalValues();

            MessengerService.Send(new MediumEditedMessage(summaryModel));
        }
        catch (Exception ex)
        {
            ValidationMessage = $"Error while saving changes: {ex.Message}";
            HasValidationErrors = true;
            Console.WriteLine($"Error while saving medium: {ex.Message}");
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private async Task GoBack()
    {
        IsMediumSelected = false;
        await _navigationService.GoToAsync("//media");
    }

    [RelayCommand]
    private async Task GoToSelect()
    {
        await _navigationService.GoToAsync("//select");
    }

    [RelayCommand]
    private async Task NavigateToSettings()
    {
        await _navigationService.GoToAsync("//settings");
    }

    public void Receive(ManagerSelectedMessage message)
    {
        _selectedManagerType = message.SelectedType;
    }

    public async void Receive(MediumSelectedMessage message)
    {
        if (message.SelectedMediumId == _mediumId && message.SelectedPlaylistId == _playlistId)
            return;

        _selectedManagerType = message.ManagerType;
        _mediumId = message.SelectedMediumId;

        await LoadPlaylistAndRelatedData(message.SelectedPlaylistId);
        await LoadMediumDetailCommand.ExecuteAsync(null);
    }

    public async void Receive(MediumEditedMessage message)
    {
        if (message.Medium.MediumId == _mediumId)
        {
            await LoadMediumDetailCommand.ExecuteAsync(null);
        }
    }

    public async void Receive(MediumRemovedMessage message)
    {
        if (message.MediumId == _mediumId)
        {
            ClearMediumSelection();
            await GoBackCommand.ExecuteAsync(null);
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
