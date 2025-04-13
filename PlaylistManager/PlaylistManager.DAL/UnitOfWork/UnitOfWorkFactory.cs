using Microsoft.EntityFrameworkCore;

namespace PlaylistManager.DAL.UnitOfWork;

public class UnitOfWorkFactory(IDbContextFactory<PlaylistManagerDbContext> dbContextFactory) : IUnitOfWorkFactory
{
    public IUnitOfWork Create()
        => new UnitOfWork(dbContextFactory.CreateDbContext());
}
