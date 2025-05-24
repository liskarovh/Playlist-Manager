using Microsoft.EntityFrameworkCore;
using PlaylistManager.Common.Enums;
using PlaylistManager.BL.Enums;
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

    public async Task<IEnumerable<PlaylistSummaryModel>> GetPlaylistsByTypeAsync(PlaylistType playlistType)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        IQueryable<PlaylistEntity> query = uow.GetRepository<PlaylistEntity, PlaylistEntityMapper>().Get();

        query = query.Where(p => p.Type == playlistType);

        query = IncludesNavigationPathDetail.Aggregate(query, (current, includePath) => current.Include(includePath));

        List<PlaylistEntity> entities = await query.ToListAsync().ConfigureAwait(false);
        return ModelMapper.MapToSummary(entities);
    }

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
    public async Task<IEnumerable<PlaylistSummaryModel>> GetPlaylistsByNameAsync(string? namePrefix, PlaylistType playlistType)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        IQueryable<PlaylistEntity> query = uow.GetRepository<PlaylistEntity, PlaylistEntityMapper>().Get();

        query = query.Where(p => p.Type == playlistType);

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

        query = IncludesNavigationPathDetail.Aggregate(query, (current, includePath) => current.Include(includePath));

        List<PlaylistEntity> entities = await query.ToListAsync().ConfigureAwait(false);
        return ModelMapper.MapToSummary(entities);
    }

    public async Task<IEnumerable<MediumSummaryModel>> GetMediaInPlaylistByTitleAsync(Guid playlistId, string mediaTitlePrefix)
    {
        // This method can now be a specific case of the new more general sorted method
        return await GetMediaInPlaylistSortedAsync(playlistId, mediaTitlePrefix, MediaSortBy.Title, SortOrder.Ascending);
    }

    // New method implementation
    public async Task<IEnumerable<PlaylistSummaryModel>> GetPlaylistsSortedAsync(PlaylistSortBy sortBy, SortOrder sortOrder, PlaylistType playlistType)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        IQueryable<PlaylistEntity> query = uow.GetRepository<PlaylistEntity, PlaylistEntityMapper>().Get();

        query = query.Where(p => p.Type == playlistType);

        query = IncludesNavigationPathDetail.Aggregate(query, (current, includePath) => current.Include(includePath));

        List<PlaylistEntity> entities = await query.ToListAsync().ConfigureAwait(false);

        IEnumerable<PlaylistSummaryModel> playlistSummaries = ModelMapper.MapToSummary(entities);

        switch (sortBy)
        {
            case PlaylistSortBy.Title:
                playlistSummaries = sortOrder == SortOrder.Ascending
                    ? playlistSummaries.OrderBy(p => p.Title, StringComparer.OrdinalIgnoreCase) // Case-insensitive title sort
                    : playlistSummaries.OrderByDescending(p => p.Title, StringComparer.OrdinalIgnoreCase);
                break;
            case PlaylistSortBy.TotalDuration:
                playlistSummaries = sortOrder == SortOrder.Ascending
                    ? playlistSummaries.OrderBy(p => p.TotalDuration ?? 0) // Handle potential nulls
                    : playlistSummaries.OrderByDescending(p => p.TotalDuration ?? 0);
                break;
            case PlaylistSortBy.MediaCount:
                playlistSummaries = sortOrder == SortOrder.Ascending
                    ? playlistSummaries.OrderBy(p => p.MediaCount)
                    : playlistSummaries.OrderByDescending(p => p.MediaCount);
                break;
            default:
                // Optional: default to a sort or throw an exception for unhandled enum
                playlistSummaries = playlistSummaries.OrderBy(p => p.Title);
                break;
        }
        return playlistSummaries.ToList(); // ToList to execute the ordering
    }

    public Task<IEnumerable<MediumSummaryModel>> GetMediaInPlaylistSortedAsync(Guid playlistId, MediaSortBy sortBy, SortOrder sortOrder) => GetMediaInPlaylistSortedAsync(playlistId, null, sortBy, sortOrder);

    // New method implementation
    public async Task<IEnumerable<MediumSummaryModel>> GetMediaInPlaylistSortedAsync(
        Guid playlistId,
        string? mediaTitlePrefix,
        MediaSortBy sortBy,
        SortOrder sortOrder)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        IQueryable<PlaylistMultimediaEntity> query = uow
            .GetRepository<PlaylistMultimediaEntity, PlaylistMultimediaEntityMapper>()
            .Get()
            .Where(pm => pm.PlaylistId == playlistId)
            .Include(pm => pm.Multimedia); // Crucial for filtering and mapping

        // Apply title prefix filter if provided
        if (!string.IsNullOrEmpty(mediaTitlePrefix))
        {
            query = query.Where(pm => pm.Multimedia != null && pm.Multimedia.Title.StartsWith(mediaTitlePrefix));
        }

        List<PlaylistMultimediaEntity> mediaEntities = await query.ToListAsync().ConfigureAwait(false);

        // Map to MediumSummaryModel before sorting, as sorting properties are on the model
        IEnumerable<MediumSummaryModel> mediumSummaries = _mediumModelMapper.MapToSummary(mediaEntities);

        // Perform sorting on the IEnumerable<MediumSummaryModel>
        switch (sortBy)
        {
            case MediaSortBy.Title:
                mediumSummaries = sortOrder == SortOrder.Ascending
                    ? mediumSummaries.OrderBy(m => m.Title, StringComparer.OrdinalIgnoreCase)
                    : mediumSummaries.OrderByDescending(m => m.Title, StringComparer.OrdinalIgnoreCase);
                break;
            case MediaSortBy.Author:
                mediumSummaries = sortOrder == SortOrder.Ascending
                    ? mediumSummaries.OrderBy(m => m.Author ?? string.Empty, StringComparer.OrdinalIgnoreCase) // Handle null author
                    : mediumSummaries.OrderByDescending(m => m.Author ?? string.Empty, StringComparer.OrdinalIgnoreCase);
                break;
            case MediaSortBy.Duration:
                mediumSummaries = sortOrder == SortOrder.Ascending
                    ? mediumSummaries.OrderBy(m => m.Duration ?? 0) // Handle null duration
                    : mediumSummaries.OrderByDescending(m => m.Duration ?? 0);
                break;
            case MediaSortBy.AddedDate:
                mediumSummaries = sortOrder == SortOrder.Ascending
                    ? mediumSummaries.OrderBy(m => m.AddedDate)
                    : mediumSummaries.OrderByDescending(m => m.AddedDate);
                break;
            default:
                // Default to sorting by title ascending if an unknown sort option is provided
                mediumSummaries = mediumSummaries.OrderBy(m => m.Title, StringComparer.OrdinalIgnoreCase);
                break;
        }

        return mediumSummaries.ToList(); // ToList to execute the ordering
    }
}

