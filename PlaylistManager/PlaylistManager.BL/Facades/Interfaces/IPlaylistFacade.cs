using PlaylistManager.BL.Models;
using PlaylistManager.DAL.Entities;

namespace PlaylistManager.BL.Facades.Interfaces;

public interface IPlaylistFacade : IFacade<PlaylistEntity, PlaylistNameOnlyModel, PlaylistSummaryModel, PlaylistSummaryModel> { }
