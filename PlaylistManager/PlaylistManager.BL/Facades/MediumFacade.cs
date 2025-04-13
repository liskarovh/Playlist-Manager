using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Mappers;
using PlaylistManager.BL.Models;
using PlaylistManager.DAL.Entities;
using PlaylistManager.DAL.Mappers;
using PlaylistManager.DAL.UnitOfWork;

namespace PlaylistManager.BL.Facades;

public class MediumFacade(
    IUnitOfWorkFactory unitOfWorkFactory,
    MediumModelMapper modelMapper)
    : FacadeBase<PlaylistMultimediaEntity, MediumNameOnlyModel, MediumSummaryModel, MediumDetailedModel,
            PlaylistMultimediaEntityMapper>(unitOfWorkFactory, modelMapper),
        IMediumFacade
{

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

}
