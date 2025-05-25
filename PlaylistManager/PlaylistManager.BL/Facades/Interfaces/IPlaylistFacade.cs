using PlaylistManager.BL.Enums;
using PlaylistManager.BL.Models;
using PlaylistManager.DAL.Entities;
using PlaylistManager.Common.Enums;

namespace PlaylistManager.BL.Facades.Interfaces;

public interface
    IPlaylistFacade : IFacade<PlaylistEntity, PlaylistNameOnlyModel, PlaylistSummaryModel, PlaylistSummaryModel>
{
    Task<PlaylistSummaryModel?> GetPlaylistByIdAsync(Guid playlistId);
    Task<IEnumerable<PlaylistSummaryModel>> GetPlaylistsByTypeAsync(PlaylistType playlistType);
    Task<IEnumerable<PlaylistSummaryModel>> GetPlaylistsByNameAsync(string? namePrefix, PlaylistType playlistType);
    Task<IEnumerable<MediumSummaryModel>> GetMediaInPlaylistByTitleAsync(Guid playlistId, string mediaTitlePrefix);
    Task<IEnumerable<PlaylistSummaryModel>> GetPlaylistsSortedAsync(PlaylistSortBy sortBy, SortOrder sortOrder, PlaylistType playlistType);
    Task<IEnumerable<MediumSummaryModel>> GetMediaInPlaylistSortedAsync(Guid playlistId, MediaSortBy sortBy, SortOrder sortOrder);

    Task<IEnumerable<MediumSummaryModel>> GetMediaInPlaylistSortedAsync(
        Guid playlistId,
        string? mediaTitlePrefix,
        MediaSortBy sortBy,
        SortOrder sortOrder);
}
