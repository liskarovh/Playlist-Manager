using System.Collections.ObjectModel;
using PlaylistManager.BL.Models;
using PlaylistManager.DAL.Entities;

namespace PlaylistManager.BL.Mappers;

public class PlaylistModelMapper(MediumModelMapper mediumModelMapper) : ModelMapperBase<PlaylistEntity, PlaylistNameOnlyModel, PlaylistSummaryModel, PlaylistSummaryModel>
{
    public override PlaylistNameOnlyModel MapToNameOnly(PlaylistEntity? entity)
        => entity == null
               ? PlaylistNameOnlyModel.Empty
               : new PlaylistNameOnlyModel
               {
                   Id = entity.Id,
                   PlaylistId = entity.Id,
                   Title = entity.Title
               };

    public override PlaylistSummaryModel MapToSummary(PlaylistEntity? entity)
    {
        if (entity == null)
        {
            return PlaylistSummaryModel.Empty;
        }

        // If the PlaylistMultimedia collection is populated, calculate the number of media and total duration.
        uint mediaCount = (uint)(entity.PlaylistMultimedia?
                                     .Count(pm => pm.Multimedia != null)
                                 ?? 0);
        double totalDuration = entity.PlaylistMultimedia?
                                     .Where(pm => pm.Multimedia != null)
                                     .Sum(pm => pm.Multimedia?.Duration ?? 0.0)
                               ?? 0.0;

        ObservableCollection<MediumDetailedModel> m = new ObservableCollection<MediumDetailedModel>();

        if (entity.PlaylistMultimedia != null)
        {
            foreach (var medium in entity.PlaylistMultimedia)
            {
                var newMedium = mediumModelMapper.MapToDetailModel(medium);
                m.Add(newMedium);
            }
        }

        return new PlaylistSummaryModel
        {
            Id = entity.Id,
            PlaylistId = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            MediaCount = mediaCount,
            TotalDuration = totalDuration,
            Medias = m
        };
    }

    public override PlaylistSummaryModel MapToDetailModel(PlaylistEntity? entity) => MapToSummary(entity); // We'll use the same mapping as for SummaryModel.

    public override PlaylistEntity MapToEntity(PlaylistSummaryModel model)
    {
        if (model.Id == Guid.Empty)
        {
            throw new ArgumentException("Model Id cannot be empty.", nameof(model.Id));
        }

        if (model.PlaylistId == Guid.Empty)
        {
            throw new ArgumentException("Model Id cannot be empty.", nameof(model.PlaylistId));
        }

        return new PlaylistEntity
        {
            Id = model.PlaylistId,
            Title = model.Title,
            Description = model.Description,
            Type = model.Type,
            PlaylistMultimedia = new List<PlaylistMultimediaEntity>() // Doesn't need to worry about possible null dereferencing.
        };
    }
}
