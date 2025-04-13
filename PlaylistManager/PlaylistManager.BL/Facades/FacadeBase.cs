using System.Collections;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PlaylistManager.DAL.Entities;
using PlaylistManager.DAL.Mappers;
using PlaylistManager.DAL.Repositories;
using PlaylistManager.DAL.UnitOfWork;
using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Models;
using PlaylistManager.BL.Mappers;

namespace PlaylistManager.BL.Facades;

public abstract class
    FacadeBase<TEntity, TNameOnlyModel, TSummaryModel, TDetailModel, TEntityMapper>(
        IUnitOfWorkFactory unitOfWorkFactory,
        IModelMapper<TEntity, TNameOnlyModel, TSummaryModel, TDetailModel> modelMapper)
    : IFacade<TEntity, TNameOnlyModel, TSummaryModel, TDetailModel>
    where TEntity : class, IEntity
    where TNameOnlyModel : IModel
    where TSummaryModel : IModel
    where TDetailModel : class, IModel
    where TEntityMapper : IEntityMapper<TEntity>, new()
{
    protected readonly IModelMapper<TEntity, TNameOnlyModel, TSummaryModel, TDetailModel> ModelMapper = modelMapper;
    protected readonly IUnitOfWorkFactory UnitOfWorkFactory = unitOfWorkFactory;

    protected virtual ICollection<string> IncludesNavigationPathDetail => new List<string>();

    public async Task DeleteAsync(Guid id)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        try
        {
            await uow.GetRepository<TEntity, TEntityMapper>().DeleteAsync(id).ConfigureAwait(false);
            await uow.CommitAsync().ConfigureAwait(false);
        }
        catch (DbUpdateException e)
        {
            throw new InvalidOperationException("Entity deletion failed.", e);
        }
    }

    public virtual async Task<TDetailModel?> GetAsync(Guid id)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        IQueryable<TEntity> query = uow.GetRepository<TEntity, TEntityMapper>().Get();

        query = IncludesNavigationPathDetail.Aggregate(query, (current, includePath) => current.Include(includePath));

        TEntity? entity = await query.SingleOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);

        return entity is null ? null : ModelMapper.MapToDetailModel(entity);
    }

    public virtual async Task<IEnumerable<TSummaryModel>> GetAsyncSummary()
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        List<TEntity> entities = await uow
                                       .GetRepository<TEntity, TEntityMapper>()
                                       .Get()
                                       .ToListAsync().ConfigureAwait(false);

        return ModelMapper.MapToSummary(entities);
    }

    public virtual async Task<IEnumerable<TNameOnlyModel>> GetAsync()
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        List<TEntity> entities = await uow
                                       .GetRepository<TEntity, TEntityMapper>()
                                       .Get()
                                       .ToListAsync().ConfigureAwait(false);

        return ModelMapper.MapToNameOnly(entities);
    }

    public virtual async Task<TDetailModel> SaveAsync(TDetailModel model)
    {
        TDetailModel result;

        GuardCollectionsAreNotSet(model);

        TEntity entity = ModelMapper.MapToEntity(model);
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        IRepository<TEntity> repository = uow.GetRepository<TEntity, TEntityMapper>();

        if (await repository.ExistsAsync(entity).ConfigureAwait(false))
        {
            TEntity updatedEntity = await repository.UpdateAsync(entity).ConfigureAwait(false);
            result = ModelMapper.MapToDetailModel(updatedEntity);
        }
        else
        {
            entity.Id = Guid.NewGuid();
            TEntity insertedEntity = repository.Insert(entity);
            result = ModelMapper.MapToDetailModel(insertedEntity);
        }

        await uow.CommitAsync().ConfigureAwait(false);

        return result;
    }

    private static void GuardCollectionsAreNotSet(TDetailModel model)
    {
        IEnumerable<PropertyInfo> collectionProperties = model
                                                         .GetType()
                                                         .GetProperties()
                                                         .Where(i => typeof(ICollection).IsAssignableFrom(i.PropertyType));

        if (collectionProperties.Any(collectionProperty => collectionProperty.GetValue(model) is ICollection { Count: > 0 }))
        {
            throw new InvalidOperationException("Current BL and DAL infrastructure disallows insert or update of models with adjacent collections.");
        }
    }
}
