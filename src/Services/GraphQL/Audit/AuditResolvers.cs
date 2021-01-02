using System;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.GraphQL.DataLoaders;
using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    public class AuditResolvers
    {
        public async Task<User?> GetUserAsync(
            AuditEvent auditEvent,
            UserByIdDataLoader userById,
            CancellationToken cancellationToken)
        {
            if (auditEvent.UserId.HasValue)
            {
                return await userById.LoadAsync(auditEvent.UserId.Value, cancellationToken);
            }

            return null;
        }

        public async Task<Media?> GetMediaAsyc(
            AuditEvent auditEvent,
            MediaByIdDataLoader mediaByid,
            CancellationToken cancellationToken)
        {
            if (auditEvent.Resource  is { } r &&
                r.Type == ProtectedResourceType.Media &&
                r.Id != null)
            {

                if ( Guid.TryParse(r.Id, out Guid mediaId))
                {
                    return await mediaByid.LoadAsync(mediaId, cancellationToken);
                }
            }

            return null;
        }

        public string? GetThumbnail(AuditEvent auditEvent)
        {
            if (auditEvent.Resource is { } r && r.Id is { })
            {
                if ( r.Type == ProtectedResourceType.Media)
                {
                    return $"/api/media/{r.Id}/thumbnail/{ThumbnailSizeName.SqXs}";
                }
                else if ( r.Type == ProtectedResourceType.Face)
                {
                    return $"/api/face/{r.Id}/thumbnail";
                }
            }

            return null;
        }
    }
}
