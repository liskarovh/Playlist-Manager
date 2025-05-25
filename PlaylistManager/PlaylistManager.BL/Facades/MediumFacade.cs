using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PlaylistManager.DAL.Entities;
using PlaylistManager.DAL.Mappers;
using PlaylistManager.DAL.UnitOfWork;
using PlaylistManager.DAL.Repositories;
using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Mappers;
using PlaylistManager.BL.Models;

namespace PlaylistManager.BL.Facades;

public class MediumFacade(IUnitOfWorkFactory unitOfWorkFactory, MediumModelMapper modelMapper)
    : FacadeBase<PlaylistMultimediaEntity, MediumNameOnlyModel, MediumSummaryModel, MediumDetailedModel, PlaylistMultimediaEntityMapper>(unitOfWorkFactory, modelMapper),
      IMediumFacade
{
    protected override ICollection<string> IncludesNavigationPathDetail
        => [$"{nameof(PlaylistMultimediaEntity.Multimedia)}"];

    private static IIncludableQueryable<PlaylistMultimediaEntity, object> Include(IQueryable<PlaylistMultimediaEntity> query)
        => query.Include(e => e.Multimedia);

    public override async Task<IEnumerable<MediumSummaryModel>> GetAsyncSummary()
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        List<PlaylistMultimediaEntity> entities = await uow
                                                        .GetRepository<PlaylistMultimediaEntity, PlaylistMultimediaEntityMapper>()
                                                        .Get(Include)
                                                        .ToListAsync().ConfigureAwait(false);

        return ModelMapper.MapToSummary(entities);
    }

    public override async Task<IEnumerable<MediumNameOnlyModel>> GetAsync()
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        List<PlaylistMultimediaEntity> entities = await uow
                                                        .GetRepository<PlaylistMultimediaEntity, PlaylistMultimediaEntityMapper>()
                                                        .Get(Include)
                                                        .ToListAsync().ConfigureAwait(false);

        return ModelMapper.MapToNameOnly(entities);
    }

    public override async Task<MediumDetailedModel?> GetAsync(Guid id)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        IQueryable<PlaylistMultimediaEntity> query = uow
                                                     .GetRepository<PlaylistMultimediaEntity, PlaylistMultimediaEntityMapper>()
                                                     .Get(Include);
        query = IncludesNavigationPathDetail.Aggregate(query, (current, includePath) => current.Include(includePath));

        PlaylistMultimediaEntity? entity = await query
                                                 .SingleOrDefaultAsync(e => e.Id == id)
                                                 .ConfigureAwait(false);

        return entity is null ? null : ModelMapper.MapToDetailModel(entity);
    }

    public async Task<IEnumerable<MediumSummaryModel>> GetMediaByPlaylistIdAsync(Guid playlistId)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        List<PlaylistMultimediaEntity> entities = await uow
                                                        .GetRepository<PlaylistMultimediaEntity, PlaylistMultimediaEntityMapper>()
                                                        .Get(Include)
                                                        .Where(e => e.PlaylistId == playlistId)
                                                        .ToListAsync().ConfigureAwait(false);

        return ModelMapper.MapToSummary(entities);
    }

    public override async Task<MediumDetailedModel> SaveAsync(MediumDetailedModel model)
    {
        if (model.PlaylistId == Guid.Empty)
        {
            throw new ArgumentException("PlaylistId must be provided in the model.", nameof(model.PlaylistId));
        }

        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<PlaylistEntity> playlistRepo = uow.GetRepository<PlaylistEntity, PlaylistEntityMapper>();

        PlaylistEntity? playlist = await playlistRepo.Get()
                                                     .SingleOrDefaultAsync(e => e.Id == model.PlaylistId)
                                                     .ConfigureAwait(false);

        if (playlist == null)
        {
            throw new InvalidOperationException($"Playlist with Id {model.PlaylistId} not found.");
        }

        if (ModelMapper is not MediumModelMapper mediumMapper)
        {
            throw new InvalidOperationException("Model mapper is not of expected type MediumModelMapper.");
        }

        PlaylistMultimediaEntity entity = mediumMapper.MapToEntity(model, playlist);
        IRepository<PlaylistMultimediaEntity> repository = uow.GetRepository<PlaylistMultimediaEntity, PlaylistMultimediaEntityMapper>();

        if (await repository.ExistsAsync(entity).ConfigureAwait(false))
        {
            PlaylistMultimediaEntity updatedEntity = await repository.UpdateAsync(entity, Include).ConfigureAwait(false);
            model = ModelMapper.MapToDetailModel(updatedEntity);
        }
        else
        {
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }

            PlaylistMultimediaEntity insertedEntity = repository.Insert(entity, Include);
            model = ModelMapper.MapToDetailModel(insertedEntity);
        }

        await uow.CommitAsync().ConfigureAwait(false);

        return model;
    }
}
