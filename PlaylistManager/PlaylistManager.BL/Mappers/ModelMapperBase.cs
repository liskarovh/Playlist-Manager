namespace PlaylistManager.BL.Mappers;

public abstract class ModelMapperBase<TEntity, TNameOnlyModel, TSummaryModel, TDetailModel>
    : IModelMapper<TEntity, TNameOnlyModel, TSummaryModel, TDetailModel>
{
    public abstract TNameOnlyModel MapToNameOnly(TEntity? entity);

    public virtual IEnumerable<TNameOnlyModel> MapToNameOnly(IEnumerable<TEntity> entities)
        => entities.Select(MapToNameOnly);

    public abstract TSummaryModel MapToSummary(TEntity entity);
    public virtual IEnumerable<TSummaryModel> MapToSummary(IEnumerable<TEntity> entities)
        => entities.Select(MapToSummary);

    public abstract TDetailModel MapToDetailModel(TEntity entity);
    public abstract TEntity MapToEntity(TDetailModel model);
}
