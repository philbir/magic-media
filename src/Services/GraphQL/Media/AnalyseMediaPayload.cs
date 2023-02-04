using MagicMedia.Store;

namespace MagicMedia.GraphQL;

public class AnalyseMediaPayload : Payload
{
    public MediaAI? MediaAI { get; }

    public AnalyseMediaPayload(MediaAI? mediaAI)
    {
        MediaAI = mediaAI;
    }

    public AnalyseMediaPayload(IReadOnlyList<UserError>? errors = null)
        : base(errors)
    {
    }
}
