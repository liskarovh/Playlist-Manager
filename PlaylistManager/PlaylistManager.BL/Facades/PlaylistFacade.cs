using Microsoft.EntityFrameworkCore;
using PlaylistManager.DAL.Entities;
using PlaylistManager.DAL.Mappers;
using PlaylistManager.DAL.UnitOfWork;
using PlaylistManager.BL.Facades.Interfaces;
using PlaylistManager.BL.Mappers;
using PlaylistManager.BL.Models;

namespace PlaylistManager.BL.Facades;

public class PlaylistFacade
    : FacadeBase<PlaylistEntity, PlaylistNameOnlyModel, PlaylistSummaryModel, PlaylistSummaryModel, PlaylistEntityMapper>,
      IPlaylistFacade
{

    private readonly MediumModelMapper _mediumModelMapper;

    protected override ICollection<string> IncludesNavigationPathDetail
        => [$"{nameof(PlaylistEntity.PlaylistMultimedia)}.{nameof(PlaylistMultimediaEntity.Multimedia)}"];

    public PlaylistFacade(
        IUnitOfWorkFactory unitOfWorkFactory,
        PlaylistModelMapper playlistModelMapper, // This is 'ModelMapper' for FacadeBase
        MediumModelMapper mediumModelMapper      // New injected dependency
    ) : base(unitOfWorkFactory, playlistModelMapper)
    {
        _mediumModelMapper = mediumModelMapper; // Store injected mapper
    }

    /// <summary>
    /// Gets playlists whose names start with the given prefix.
    /// If the prefix is null or empty, all playlists are returned.
    /// </summary>
    /// <param name="namePrefix">The prefix to filter playlist names by.</param>
    /// <returns>A collection of playlist summaries matching the criteria.</returns>
    public async Task<IEnumerable<PlaylistSummaryModel>> GetPlaylistsByNameAsync(string? namePrefix)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        IQueryable<PlaylistEntity> query = uow.GetRepository<PlaylistEntity, PlaylistEntityMapper>().Get();


        // Apply includes necessary for PlaylistSummaryModel mapping
        query = IncludesNavigationPathDetail.Aggregate(query, (current, includePath) => current.Include(includePath));
        if (!string.IsNullOrEmpty(namePrefix))
        {
            query = query.Where(p => p.Title.StartsWith(namePrefix));
        }

        List<PlaylistEntity> entities = await query.ToListAsync().ConfigureAwait(false);
        return ModelMapper.MapToSummary(entities);
    }

    public override async Task<IEnumerable<PlaylistSummaryModel>> GetAsyncSummary()
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        IQueryable<PlaylistEntity> query = uow.GetRepository<PlaylistEntity, PlaylistEntityMapper>().Get();

        // Apply includes necessary for PlaylistSummaryModel mapping,
        // as PlaylistModelMapper.MapToSummary depends on PlaylistMultimedia and its Multimedia.
        query = IncludesNavigationPathDetail.Aggregate(query, (current, includePath) => current.Include(includePath));

        List<PlaylistEntity> entities = await query.ToListAsync().ConfigureAwait(false);
        return ModelMapper.MapToSummary(entities);
    }

    public async Task<IEnumerable<MediumSummaryModel>> GetMediaInPlaylistByTitleAsync(Guid playlistId, string mediaTitlePrefix)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        IQueryable<PlaylistMultimediaEntity> query = uow
            .GetRepository<PlaylistMultimediaEntity, PlaylistMultimediaEntityMapper>()
            .Get()
            .Where(pm => pm.PlaylistId == playlistId)
            .Include(pm => pm.Multimedia);

        if (!string.IsNullOrEmpty(mediaTitlePrefix))
        {
            query = query.Where(pm => pm.Multimedia != null && pm.Multimedia.Title.StartsWith(mediaTitlePrefix));
        }

        List<PlaylistMultimediaEntity> mediaEntities = await query
            .OrderBy(pm => pm.Multimedia!.Title)
            .ToListAsync().ConfigureAwait(false);

        // Use the injected _mediumModelMapper
        return _mediumModelMapper.MapToSummary(mediaEntities);
    }
}
