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
        PlaylistModelMapper playlistModelMapper,
        MediumModelMapper mediumModelMapper
    ) : base(unitOfWorkFactory, playlistModelMapper)
    {
        _mediumModelMapper = mediumModelMapper;
    }

    public async Task<PlaylistSummaryModel?> GetPlaylistByIdAsync(Guid playlistId)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        IQueryable<PlaylistEntity> query = uow.GetRepository<PlaylistEntity, PlaylistEntityMapper>().Get();

        query = query.Where(p => p.Id == playlistId);

        query = IncludesNavigationPathDetail.Aggregate(query, (current, includePath) => current.Include(includePath));

        PlaylistEntity? entity = await query.FirstOrDefaultAsync().ConfigureAwait(false);

        if (entity == null)
        {
            return null;
        }

        return ModelMapper.MapToSummary(entity);
    }

    public async Task<IEnumerable<PlaylistSummaryModel>> GetPlaylistsByNameAsync(string? namePrefix, PlaylistType playlistType)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();
        IQueryable<PlaylistEntity> query = uow.GetRepository<PlaylistEntity, PlaylistEntityMapper>().Get();

        query = query.Where(p => p.Type == playlistType);

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
                playlistSummaries = playlistSummaries.OrderBy(p => p.Title);
                break;
        }
        return playlistSummaries.ToList();
    }


    public async Task<IEnumerable<MediumSummaryModel>> GetMediaInPlaylistSortedAsync(
        Guid playlistId,
        MediaFilterBy? filterBy,
        string? filterValue,
        MediaSortBy sortBy = MediaSortBy.Title,
        SortOrder sortOrder = SortOrder.Descending)
    {
        await using IUnitOfWork uow = UnitOfWorkFactory.Create();

        IQueryable<PlaylistMultimediaEntity> query = uow
            .GetRepository<PlaylistMultimediaEntity, PlaylistMultimediaEntityMapper>()
            .Get()
            .Where(pm => pm.PlaylistId == playlistId)
            .Include(pm => pm.Multimedia); // Crucial for filtering on Multimedia properties and mapping

        // Apply property filter if filterBy and filterValue are provided and valid
        if (filterBy.HasValue && !string.IsNullOrEmpty(filterValue))
        {
            // Ensure Multimedia is not null before accessing its properties
            query = query.Where(pm => pm.Multimedia != null);

            switch (filterBy.Value)
            {
                case MediaFilterBy.Title:
                    query = query.Where(pm => pm.Multimedia != null && pm.Multimedia.Title.StartsWith(filterValue));
                    break;
                case MediaFilterBy.Author:
                    // Author is nullable on MultimediaBaseEntity
                    query = query.Where(pm => pm.Multimedia!.Author != null && pm.Multimedia.Author.StartsWith(filterValue));
                    break;
            }
        }

        List<PlaylistMultimediaEntity> mediaEntities = await query.ToListAsync().ConfigureAwait(false);

        IEnumerable<MediumSummaryModel> mediumSummaries = _mediumModelMapper.MapToSummary(mediaEntities);

        switch (sortBy)
        {
            case MediaSortBy.Title:
                mediumSummaries = sortOrder == SortOrder.Ascending
                    ? mediumSummaries.OrderBy(m => m.Title, StringComparer.OrdinalIgnoreCase)
                    : mediumSummaries.OrderByDescending(m => m.Title, StringComparer.OrdinalIgnoreCase);
                break;
            case MediaSortBy.Author:
                mediumSummaries = sortOrder == SortOrder.Ascending
                    ? mediumSummaries.OrderBy(m => m.Author ?? string.Empty, StringComparer.OrdinalIgnoreCase)
                    : mediumSummaries.OrderByDescending(m => m.Author ?? string.Empty, StringComparer.OrdinalIgnoreCase);
                break;
            case MediaSortBy.Duration:
                mediumSummaries = sortOrder == SortOrder.Ascending
                    ? mediumSummaries.OrderBy(m => m.Duration ?? 0)
                    : mediumSummaries.OrderByDescending(m => m.Duration ?? 0);
                break;
            case MediaSortBy.AddedDate:
                mediumSummaries = sortOrder == SortOrder.Ascending
                    ? mediumSummaries.OrderBy(m => m.AddedDate)
                    : mediumSummaries.OrderByDescending(m => m.AddedDate);
                break;
            default:
                mediumSummaries = mediumSummaries.OrderBy(m => m.Title, StringComparer.OrdinalIgnoreCase);
                break;
        }

        return mediumSummaries.ToList();
    }
}

