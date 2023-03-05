using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Extensions;
using MagicMedia.Security;
using MagicMedia.Store;

namespace MagicMedia;

public class PersonTimelineService : IPersonTimelineService
{
    private readonly IMediaStore _store;
    private readonly IUserAuthorizationService _userAuthorizationService;

    public PersonTimelineService(
        IMediaStore store,
        IUserAuthorizationService userAuthorizationService)
    {
        _store = store;
        _userAuthorizationService = userAuthorizationService;
    }

    public async Task<PersonTimeline> BuildTimelineAsync(
        Guid personId,
        int itemsPerYear,
        CancellationToken cancellationToken)
    {
        var ages = new List<PersonTimelineAge>();

        //Person person = await _store.Persons.GetByIdAsync(personId, cancellationToken);

        UserResourceAccessInfo authInfo = await _userAuthorizationService.GetAuthorizedOnAsync(
            ProtectedResourceType.Face,
            cancellationToken);

        IEnumerable<MediaFace> faces = await _store.Faces.GetFacesByPersonAsync(
            personId,
            authInfo.ViewAll ? null : authInfo.Ids,
            cancellationToken);

        IEnumerable<IGrouping<int, MediaFace>>? byYear = faces
            .OrderBy(x => x.Age)
            .Where(x => x.Age.HasValue && x.State == FaceState.Validated)
            .GroupBy(x => (int)(x.Age!.Value / 12));

        foreach (IGrouping<int, MediaFace>? yaer in byYear)
        {
            var facesByYaer = new List<MediaFace>();

            MediaFace[] sorted = yaer.OrderBy(x => x.Age)
                .ThenByDescending(x => x.Box.GetResolution())
                .ToArray();

            var step = sorted.Count() / itemsPerYear;
            step = step > 0 ? step : 1;

            for (int i = 0; i < Math.Min(itemsPerYear, sorted.Count()); i++)
            {
                facesByYaer.Add(sorted[i * step]);
            }

            ages.Add(new PersonTimelineAge { Age = yaer.Key, Faces = facesByYaer });
        }

        return new PersonTimeline { Ages = ages };
    }
}
