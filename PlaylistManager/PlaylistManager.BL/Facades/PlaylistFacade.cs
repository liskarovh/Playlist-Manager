using PlaylistManager.DAL.Entities;
using PlaylistManager.DAL.Mappers;
using PlaylistManager.DAL.UnitOfWork;
using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Mappers;
using PlaylistManager.BL.Models;

namespace PlaylistManager.BL.Facades;

public class PlaylistFacade(IUnitOfWorkFactory unitOfWorkFactory, PlaylistModelMapper modelMapper)
    : FacadeBase<PlaylistEntity, PlaylistNameOnlyModel, PlaylistSummaryModel, PlaylistSummaryModel, PlaylistEntityMapper>(unitOfWorkFactory, modelMapper),
      IPlaylistFacade
{
    protected override ICollection<string> IncludesNavigationPathDetail
        => [$"{nameof(PlaylistEntity.PlaylistMultimedia)}.{nameof(PlaylistMultimediaEntity.Multimedia)}"];
}
