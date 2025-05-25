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

    public async Task<MediumDetailedModel?> GetMediumByIdAsync(Guid mediumId)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        IQueryable<PlaylistMultimediaEntity> query = uow
                                                     .GetRepository<PlaylistMultimediaEntity, PlaylistMultimediaEntityMapper>()
                                                     .Get(Include);

        query = IncludesNavigationPathDetail.Aggregate(query, (current, includePath) => current.Include(includePath));

        PlaylistMultimediaEntity? entity = await query
                                                 .SingleOrDefaultAsync(e => e.MultimediaId == mediumId)
                                                 .ConfigureAwait(false);

        return entity is null ? null : ModelMapper.MapToDetailModel(entity);
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
        // This check might be overly defensive if DI guarantees the correct type.
        // However, if ModelMapper is the base interface type, this cast is necessary
        // for the specialized MapToEntity overload.
        throw new InvalidOperationException("Model mapper is not of expected type MediumModelMapper.");
    }

    PlaylistMultimediaEntity entity = mediumMapper.MapToEntity(model, playlist);
    IRepository<PlaylistMultimediaEntity> repository = uow.GetRepository<PlaylistMultimediaEntity, PlaylistMultimediaEntityMapper>();

    PlaylistMultimediaEntity entityToMap; // Declare here to use in both branches

    if (await repository.ExistsAsync(entity).ConfigureAwait(false)) // Assumes ExistsAsync checks by Id or a unique key
    {
        // UPDATE PATH
        // Ensure the entity being updated has the correct Id from the model
        // The mapper should have set entity.Id = model.Id
        // If ExistsAsync checks by more than Id, ensure entity is correctly populated for that check.

        PlaylistMultimediaEntity updatedEntity = await repository.UpdateAsync(entity, Include).ConfigureAwait(false);
        // 'Include' here is appropriate for UpdateAsync if it re-fetches or ensures navigation properties are loaded.
        entityToMap = updatedEntity;
    }
    else
    {
        // INSERT PATH
        if (entity.Id == Guid.Empty) // This is for PlaylistMultimediaEntity.Id
        {
            entity.Id = Guid.NewGuid();
        }
        // model.MediumId is used for entity.Multimedia.Id and entity.MultimediaId by the mapper.
        // The mapper already throws if model.MediumId is Guid.Empty.

        // Call Insert without the 'Include' delegate.
        // The repository.Insert should just add the entity to the DbContext.
        repository.Insert(entity); // No 'Include' needed for the act of insertion.
                                   // This typically returns the tracked entity.

        // The 'entity' instance is now tracked as 'Added'.
        // Its 'Multimedia' navigation property and 'MultimediaId' FK are set.
        // EF Core will handle inserting both PlaylistMultimediaEntity and its related MultimediaBaseEntity.

        // To ensure we map from the state persisted in the database (especially if there are DB-generated values
        // or triggers, though less common with Guids unless specifically configured),
        // we will re-fetch after commit. For now, we'll use the 'entity' instance.
        // The actual database IDs will be fixed up by SaveChangesAsync.
        entityToMap = entity; // Use the entity that was prepared for insert
    }

    await uow.CommitAsync().ConfigureAwait(false);

    // AFTER COMMIT: Re-fetch the entity to ensure we have the definitive state from the database,
    // especially for the insert path, to correctly load navigation properties for mapping.
    // This is crucial if the 'entityToMap' from the insert path doesn't have its navigation
    // properties fully realized for mapping in the same way an update might.

    // Use the ID of the entity that was committed.
    Guid committedEntityId = entityToMap.Id;
    PlaylistMultimediaEntity? finalEntity = await repository.Get(Include) // Use the same Include as GetAsync
                                                           .SingleOrDefaultAsync(e => e.Id == committedEntityId)
                                                           .ConfigureAwait(false);

    if (finalEntity == null)
    {
        // This would be unexpected if commit was successful
        throw new InvalidOperationException("Failed to retrieve the entity after save operation.");
    }

    return ModelMapper.MapToDetailModel(finalEntity);
}
}
