using PlaylistManager.BL.Models;
using PlaylistManager.DAL.Entities;

namespace PlaylistManager.BL.Facades.Interfaces;

public interface IMediumFacade : IFacade<PlaylistMultimediaEntity, MediumNameOnlyModel, MediumSummaryModel, MediumDetailedModel> { }
