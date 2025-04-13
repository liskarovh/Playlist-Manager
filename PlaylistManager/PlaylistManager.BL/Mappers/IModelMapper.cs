namespace PlaylistManager.BL.Mappers;

public interface IModelMapper<TEntity, out TNameOnlyModel, out TSummaryModel, TDetailModel>
{
    TNameOnlyModel MapToNameOnly(TEntity? entity);
    IEnumerable<TNameOnlyModel> MapToNameOnly(IEnumerable<TEntity> entities)
        => entities.Select(MapToNameOnly);

    TSummaryModel MapToSummary(TEntity entity);
    IEnumerable<TSummaryModel> MapToSummary(IEnumerable<TEntity> entities)
        => entities.Select(MapToSummary);

    TDetailModel MapToDetailModel(TEntity entity);
    TEntity MapToEntity(TDetailModel model);
}
