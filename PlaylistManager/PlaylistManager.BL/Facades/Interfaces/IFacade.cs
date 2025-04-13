using PlaylistManager.BL.Models;
using PlaylistManager.DAL.Entities;

namespace PlaylistManager.BL.Facades.Interfaces;

public interface IFacade<TEntity, TNameOnlyModel, TSummaryModel, TDetailModel>
    where TEntity : class, IEntity
    where TNameOnlyModel : IModel
    where TSummaryModel : IModel
    where TDetailModel : class, IModel
{
    Task<TDetailModel?> GetAsync(Guid id);
    Task<IEnumerable<TNameOnlyModel>> GetAsync();
    Task<IEnumerable<TSummaryModel>> GetAsyncSummary();

    Task<TDetailModel> SaveAsync(TDetailModel model);
    Task DeleteAsync(Guid id);
}
