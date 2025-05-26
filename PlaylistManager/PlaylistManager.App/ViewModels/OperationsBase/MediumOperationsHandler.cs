using System.Collections.ObjectModel;
using PlaylistManager.App.Messages;
using PlaylistManager.App.Services;
using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Models;
using PlaylistManager.BL.Enums;

namespace PlaylistManager.App.ViewModels;

public class MediumOperationsHandler(IPlaylistFacade playlistFacade,
                                     IMediumFacade mediumFacade,
                                     IMessengerService messengerService)
{
    private Guid _playlistId;

    public ObservableCollection<MediumSummaryModel> Media { get; set; } = new();
    public string MediaSearchQuery { get; set; } = string.Empty;
    public string AuthorFilterQuery { get; set; } = string.Empty;

    public string SelectedMediaSortOption { get; set; } = "Title";
    public SortOrder MediaSortOrder { get; set; } = SortOrder.Ascending;
    public string MediaSortOrderSymbol { get; set; } = "↑";
    public ObservableCollection<string> MediaSortOptions { get; } = new(["Title", "Author", "Added Date", "Duration"]);

    public void SetCurrentPlaylistId(Guid playlistId)
    {
        _playlistId = playlistId;
    }

    public async Task LoadMediaAsync()
    {
        if (_playlistId == Guid.Empty) return;

        var media = await playlistFacade.GetMediaInPlaylistSortedAsync(
                                                                        _playlistId,
                                                                        null,
                                                                        null,
                                                                        MediaSortBy.Title,
                                                                        MediaSortOrder);

        Media.Clear();
        foreach (var medium in media)
        {
            Media.Add(medium);
        }

        await SortMediaAsync(SelectedMediaSortOption);
    }

    public void ClearMedia()
    {
        Media.Clear();
        MediaSearchQuery = string.Empty;
        AuthorFilterQuery = string.Empty;
    }

    public async Task SortMediaAsync(string? sortOption)
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

        var sortedMedia = await playlistFacade.GetMediaInPlaylistSortedAsync(
                                                                              _playlistId,
                                                                              null,
                                                                              null,
                                                                              sortBy,
                                                                              MediaSortOrder);

        Media.Clear();
        foreach (var medium in sortedMedia)
        {
            Media.Add(medium);
        }
    }

    public void ToggleMediaSortOrder()
    {
        MediaSortOrder = MediaSortOrder == SortOrder.Ascending
                             ? SortOrder.Descending
                             : SortOrder.Ascending;

        MediaSortOrderSymbol = MediaSortOrder == SortOrder.Ascending
                                   ? "↑"
                                   : "↓";
    }

    public async Task SearchMediaAsync(string? searchQuery)
    {
        if (_playlistId == Guid.Empty) return;

        IEnumerable<MediumSummaryModel> results;

        if (string.IsNullOrEmpty(searchQuery))
        {
            results = await playlistFacade.GetMediaInPlaylistSortedAsync(
                                                                          _playlistId,
                                                                          null,
                                                                          null,
                                                                          MediaSortBy.Title,
                                                                          MediaSortOrder);
        }
        else
        {
            results = await playlistFacade.GetMediaInPlaylistSortedAsync(
                                                                          _playlistId,
                                                                          MediaFilterBy.Title,
                                                                          searchQuery,
                                                                          MediaSortBy.Title,
                                                                          MediaSortOrder);
        }

        Media.Clear();
        foreach (var medium in results)
        {
            Media.Add(medium);
        }
    }

    public async Task FilterMediaByAuthorAsync(string? filterQuery)
    {
        if (_playlistId == Guid.Empty) return;

        IEnumerable<MediumSummaryModel> results;

        if (string.IsNullOrEmpty(filterQuery))
        {
            results = await playlistFacade.GetMediaInPlaylistSortedAsync(
                                                                          _playlistId,
                                                                          null,
                                                                          null,
                                                                          MediaSortBy.Title,
                                                                          MediaSortOrder);
        }
        else
        {
            results = await playlistFacade.GetMediaInPlaylistSortedAsync(
                                                                          _playlistId,
                                                                          MediaFilterBy.Author,
                                                                          filterQuery,
                                                                          MediaSortBy.Title,
                                                                          MediaSortOrder);
        }

        Media.Clear();
        foreach (var medium in results)
        {
            Media.Add(medium);
        }
    }

    public async Task CreateMediumAsync()
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

            await mediumFacade.SaveAsync(newMedium);

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
            messengerService.Send(new MediumAddedMessage(summaryMedium, _playlistId));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error creating medium: {ex.Message}");
        }
    }

    public async Task DeleteMediumAsync(MediumSummaryModel medium)
    {
        if (medium == null) return;

        await mediumFacade.DeleteAsync(medium.Id);
        Media.Remove(medium);

        messengerService.Send(new MediumRemovedMessage(medium.Id));
    }

    public void HandleMediumAddedMessage(MediumAddedMessage message)
    {
        if (message.PlaylistId == _playlistId)
        {
            Media.Add(message.Medium);
            SortMediaAsync(SelectedMediaSortOption).ConfigureAwait(false);
        }
    }

    public void HandleMediumRemovedMessage(MediumRemovedMessage message)
    {
        var medium = Media.FirstOrDefault(m => m.Id == message.MediumId);
        if (medium != null)
        {
            Media.Remove(medium);
        }
    }

    public void HandleMediumEditedMessage(MediumEditedMessage message)
    {
        var index = Media.ToList().FindIndex(m => m.Id == message.Medium.Id);
        if (index >= 0)
        {
            Media[index] = message.Medium;
            SortMediaAsync(SelectedMediaSortOption).ConfigureAwait(false);
        }
    }
}
