using PlaylistManager.BL.Enums;
using PlaylistManager.BL.Models;
using PlaylistManager.DAL.Entities;

namespace PlaylistManager.BL.Facades.Interfaces;

public interface
    IPlaylistFacade : IFacade<PlaylistEntity, PlaylistNameOnlyModel, PlaylistSummaryModel, PlaylistSummaryModel>
{
    Task<IEnumerable<PlaylistSummaryModel>> GetPlaylistsByNameAsync(string? namePrefix);
    Task<IEnumerable<MediumSummaryModel>> GetMediaInPlaylistByTitleAsync(Guid playlistId, string mediaTitlePrefix);
    Task<IEnumerable<PlaylistSummaryModel>> GetPlaylistsSortedAsync(PlaylistSortBy sortBy, SortOrder sortOrder);
    Task<IEnumerable<MediumSummaryModel>> GetMediaInPlaylistSortedAsync(Guid playlistId, MediaSortBy sortBy, SortOrder sortOrder);
}
