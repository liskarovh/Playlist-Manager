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
public partial class MediumSelectedViewModel : PlaylistBaseViewModel,
                                               IRecipient<MediumAddedMessage>,
                                               IRecipient<MediumEditedMessage>,
                                               IRecipient<MediumRemovedMessage>,
                                               IRecipient<MediumSelectedMessage>
{
    private readonly MediumOperationsHandler _mediumHandler;
    private readonly IMediumFacade _mediumFacade;
    private Guid _playlistId;
    private bool _isPlaylistSelected;

    private Guid _mediumId;
    private bool _isEditMode;

    private string _originalName = string.Empty;
    private string? _originalAuthor = string.Empty;
    private string? _originalDescription = string.Empty;
    private string? _originalURL = string.Empty;
    private string _originalDuration = string.Empty;
    private string _originalReleaseYear = string.Empty;
    private string _originalFormat = string.Empty;
    private string _originalGenre = string.Empty;

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

    public bool IsMediumSelected { get; set; }
    public string MediumName { get; set; } = string.Empty;
    public Guid PlaylistMultimediaId { get; set; } = Guid.Empty;
    public Guid MediumID { get; set; } = Guid.Empty;
    public string? MediumAuthor { get; set; } = string.Empty;
    public string? MediumDescription { get; set; } = string.Empty;
    public string? MediumURL { get; set; } = string.Empty;
    public string MediumDuration { get; set; } = string.Empty;
    public string MediumReleaseYear { get; set; } = string.Empty;
    public string MediumFormat { get; set; } = string.Empty;
    public string MediumGenre { get; set; } = string.Empty;

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
            return SelectedManagerType switch
            {
                ManagerType.Video => new(new[]
                {
                    "Mp4",
                    "Avi",
                    "Mkv",
                    "Mov",
                    "None"
                }),

                ManagerType.Music or ManagerType.AudioBook => new(new[]
                {
                    "Mp3",
                    "Wav",
                    "Flac",
                    "Aac",
                    "None"
                }),

                _ => new(["None"])
            };
        }
    }

    public ObservableCollection<string> GenreOptions
    {
        get
        {
            return SelectedManagerType switch
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
                    "None"
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
                    "None"
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
                    "None"
                }),

                _ => new(["None"])
            };
        }
    }

    public MediumSelectedViewModel(
        IPlaylistFacade playlistFacade,
        IMediumFacade mediumFacade,
        INavigationService navigationService,
        IMessengerService messengerService)
        : base(playlistFacade, navigationService, messengerService)
    {
        _mediumFacade = mediumFacade;
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
                PlaylistMultimediaId = medium.Id;
                MediumID = medium.MediumId;
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
    private async Task SelectMedium(MediumSummaryModel? medium)
    {
        if (medium == null) return;

        if (medium.MediumId == _mediumId && IsMediumSelected)
            return;

        _mediumId = medium.MediumId;

        await LoadMediumDetailCommand.ExecuteAsync(null);

        MessengerService.Send(new MediumSelectedMessage(_playlistId, medium.MediumId, SelectedManagerType));
    }

    [RelayCommand]
    private async Task CreateMedium()
    {
        await _mediumHandler.CreateMediumAsync();

        var newMedium = Media.LastOrDefault();
        if (newMedium != null)
        {
            await SelectMediumCommand.ExecuteAsync(newMedium);
            StartEditCommand.Execute(null);
        }
    }

    [RelayCommand]
    private async Task DeleteCurrentMedium()
    {
        if (!IsMediumSelected || _mediumId == Guid.Empty)
            return;

        var medium = Media.FirstOrDefault(m => m.MediumId == _mediumId);
        if (medium == null) return;

        await _mediumHandler.DeleteMediumAsync(medium);
        ClearMediumSelection();
    }

    [RelayCommand]
    private async Task SelectPlaylist(PlaylistSummaryModel? playlist)
    {
        if (playlist == null) return;

        ClearMediumSelection();

        await LoadPlaylistAndRelatedData(playlist.PlaylistId);
        _playlistId = playlist.PlaylistId;

        MessengerService.Send(new PlaylistSelectedMessage(playlist));
    }

    [RelayCommand]
    private async Task CreatePlaylist()
    {
        var savedPlaylist = await CreatePlaylistInternal();
        MessengerService.Send(new PlaylistAddMessage(savedPlaylist));
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
                Id = PlaylistMultimediaId,
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
            };

            var savedModel = await _mediumFacade.SaveAsync(mediumToSave);

            IsEditMode = false;

            var summaryModel = new MediumSummaryModel
            {
                Id = savedModel.Id,
                MediumId = savedModel.MediumId,
                PlaylistId = savedModel.PlaylistId,
                Title = savedModel.Title,
                Author = savedModel.Author,
                Duration = savedModel.Duration,
                AddedDate = savedModel.AddedDate
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
        await NavigationService.GoToAsync("/media");
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

    public async void Receive(MediumSelectedMessage message)
    {
        if (message.SelectedMediumId == _mediumId && message.SelectedPlaylistId == _playlistId)
            return;

        SelectedManagerType = message.ManagerType;
        _mediumId = message.SelectedMediumId;

        OnPropertyChanged(nameof(FormatOptions));
        OnPropertyChanged(nameof(GenreOptions));

        await LoadPlaylistAndRelatedData(message.SelectedPlaylistId);
        await LoadMediumDetailCommand.ExecuteAsync(null);
    }

    public async void Receive(MediumEditedMessage message)
    {
        _mediumHandler.HandleMediumEditedMessage(message);

        if (message.Medium.MediumId == _mediumId)
        {
            await LoadMediumDetailCommand.ExecuteAsync(null);
        }
    }

    public void Receive(MediumRemovedMessage message)
    {
        _mediumHandler.HandleMediumRemovedMessage(message);

        if (message.MediumId == _mediumId)
        {
            ClearMediumSelection();
        }

        _mediumId = Guid.Empty;
    }

    public void Receive(MediumAddedMessage message)
    {
        _mediumHandler.HandleMediumAddedMessage(message);
    }
}
