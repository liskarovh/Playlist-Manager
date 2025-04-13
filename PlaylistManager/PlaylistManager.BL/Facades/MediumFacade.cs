using Microsoft.EntityFrameworkCore;
using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Mappers;
using PlaylistManager.BL.Models;
using PlaylistManager.DAL.Entities;
using PlaylistManager.DAL.Mappers;
using PlaylistManager.DAL.UnitOfWork;
using PlaylistManager.DAL.Repositories;

namespace PlaylistManager.BL.Facades;

public class MediumFacade(IUnitOfWorkFactory unitOfWorkFactory,
                          MediumModelMapper modelMapper)
    : FacadeBase<PlaylistMultimediaEntity, MediumNameOnlyModel, MediumSummaryModel, MediumDetailedModel, PlaylistMultimediaEntityMapper>(unitOfWorkFactory, modelMapper),
      IMediumFacade
{
    protected override ICollection<string> IncludesNavigationPathDetail => new[]
    {
        $"{nameof(PlaylistMultimediaEntity.Playlist)}",
        $"{nameof(PlaylistMultimediaEntity.Multimedia)}"
    };


    public override async Task<IEnumerable<MediumSummaryModel>> GetAsyncSummary()
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        Func<IQueryable<PlaylistMultimediaEntity>, IIncludableQueryable<PlaylistMultimediaEntity, object>> include =
            query => query.Include(e => e.Multimedia);

        List<PlaylistMultimediaEntity> entities = await uow
            .GetRepository<PlaylistMultimediaEntity, PlaylistMultimediaEntityMapper>()
            .Get(include)
            .ToListAsync().ConfigureAwait(false);

        return ModelMapper.MapToSummary(entities);
    }

    public override async Task<IEnumerable<MediumNameOnlyModel>> GetAsync()
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        Func<IQueryable<PlaylistMultimediaEntity>, IIncludableQueryable<PlaylistMultimediaEntity, object>> include =
            query => query.Include(e => e.Multimedia);

        List<PlaylistMultimediaEntity> entities = await uow
            .GetRepository<PlaylistMultimediaEntity, PlaylistMultimediaEntityMapper>()
            .Get(include)
            .ToListAsync().ConfigureAwait(false);

        return ModelMapper.MapToNameOnly(entities);
    }

    public override async Task<MediumDetailedModel?> GetAsync(Guid id)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        Func<IQueryable<PlaylistMultimediaEntity>, IIncludableQueryable<PlaylistMultimediaEntity, object>> include =
            query => query.Include(e => e.Multimedia);

        IQueryable<PlaylistMultimediaEntity> query = uow.GetRepository<PlaylistMultimediaEntity, PlaylistMultimediaEntityMapper>().Get(include);

        foreach (string includePath in IncludesNavigationPathDetail)
        {
            query = query.Include(includePath);
        }

        PlaylistMultimediaEntity? entity = await query.SingleOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);

        return entity is null
            ? null
            : ModelMapper.MapToDetailModel(entity);
    }

    public override async Task<MediumDetailedModel> SaveAsync(MediumDetailedModel model)
    {
        // Ověříme, že model obsahuje platné PlaylistId
        if (model.PlaylistId == Guid.Empty)
        {
            throw new ArgumentException("PlaylistId must be provided in the model.", nameof(model.PlaylistId));
        }

        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        // Získáme repozitář pro PlaylistEntity a načteme entitu dle model.PlaylistId pomocí filtru
        IRepository<PlaylistEntity> playlistRepo = uow.GetRepository<PlaylistEntity, PlaylistEntityMapper>();
        PlaylistEntity? playlist = await playlistRepo.Get()
                                                     .SingleOrDefaultAsync(e => e.Id == model.PlaylistId)
                                                     .ConfigureAwait(false);

        if (playlist == null)
        {
            throw new InvalidOperationException($"Playlist with Id {model.PlaylistId} not found.");
        }

        // Použijeme přetíženou metodu MapToEntity, která přijímá PlaylistEntity
        var mediumMapper = ModelMapper as MediumModelMapper;

        if (mediumMapper == null)
        {
            throw new InvalidOperationException("Model mapper is not of expected type MediumModelMapper.");
        }
        PlaylistMultimediaEntity entity = mediumMapper.MapToEntity(model, playlist);

        // Získáme repozitář pro PlaylistMultimediaEntity
        IRepository<PlaylistMultimediaEntity> repository = uow.GetRepository<PlaylistMultimediaEntity, PlaylistMultimediaEntityMapper>();

        // Pokud entita již existuje (update), provedeme update, jinak insert
        if (await repository.ExistsAsync(entity).ConfigureAwait(false))
        {
            PlaylistMultimediaEntity updatedEntity = await repository.UpdateAsync(entity).ConfigureAwait(false);
            model = ModelMapper.MapToDetailModel(updatedEntity);
        }
        else
        {
            // U insertu vygenerujeme nové Id, pokud není nastaveno
            if (entity.Id == Guid.Empty)
            {
                entity.Id = Guid.NewGuid();
            }
            PlaylistMultimediaEntity insertedEntity = repository.Insert(entity);
            model = ModelMapper.MapToDetailModel(insertedEntity);
        }

        await uow.CommitAsync().ConfigureAwait(false);
        return model;
    }
}

