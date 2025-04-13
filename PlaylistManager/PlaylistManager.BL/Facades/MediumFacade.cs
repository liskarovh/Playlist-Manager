using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Mappers;
using PlaylistManager.BL.Models;
using PlaylistManager.DAL.Entities;
using PlaylistManager.DAL.Mappers;
using PlaylistManager.DAL.UnitOfWork;

namespace PlaylistManager.BL.Facades;

public class MediumFacade(IUnitOfWorkFactory unitOfWorkFactory,
                          MediumModelMapper modelMapper)
    : FacadeBase<PlaylistMultimediaEntity, MediumNameOnlyModel, MediumSummaryModel, MediumDetailedModel, PlaylistMultimediaEntityMapper>(unitOfWorkFactory, modelMapper),
      IMediumFacade { }
