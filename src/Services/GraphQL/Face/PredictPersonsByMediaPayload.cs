using System.Collections.Generic;
using MagicMedia.Store;

namespace MagicMedia.GraphQL.Face
{
    public class PredictPersonsByMediaPayload : Payload
    {
        public int MatchCount { get; }

        public Media? Media { get; }

        public PredictPersonsByMediaPayload(int matchCount, Media media)
        {
            MatchCount = matchCount;
            Media = media;
        }

        public PredictPersonsByMediaPayload(IReadOnlyList<UserError>? errors = null)
            : base(errors)
        {
        }
    }
}
