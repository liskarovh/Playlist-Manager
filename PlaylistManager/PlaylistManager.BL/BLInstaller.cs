using Microsoft.Extensions.DependencyInjection;
using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Mappers;
using PlaylistManager.DAL.UnitOfWork;

namespace PlaylistManager.BL;

public static class BLInstaller
{
    public static IServiceCollection AddBLServices(this IServiceCollection services)
    {
        services.AddSingleton<IUnitOfWorkFactory, UnitOfWorkFactory>();

        // Registrace všech fasád
        services.Scan(selector => selector
                                  .FromAssemblyOf<BusinessLogic>()
                                  .AddClasses(filter => filter.AssignableTo(typeof(IFacade<,,,>)))
                                  .AsMatchingInterface()
                                  .WithSingletonLifetime());

        // Registrace všech mapperů
        services.Scan(selector => selector
                                  .FromAssemblyOf<BusinessLogic>()
                                  .AddClasses(filter => filter.AssignableTo(typeof(IModelMapper<,,,>)))
                                  .AsSelfWithInterfaces()
                                  .WithSingletonLifetime());

        return services;
    }
}
