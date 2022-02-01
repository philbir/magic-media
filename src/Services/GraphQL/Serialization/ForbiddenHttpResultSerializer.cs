using System.Net;
using HotChocolate.AspNetCore.Serialization;
using HotChocolate.Execution;

namespace MagicMedia.GraphQL;

public class ForbiddenHttpResultSerializer : DefaultHttpResultSerializer
{
    public override HttpStatusCode GetStatusCode(IExecutionResult result)
    {
        if (result is IQueryResult queryResult &&
            queryResult.Errors?.Count > 0 &&
            queryResult.Errors.Any(error => error.Code == "AUTH_NOT_AUTHENTICATED"))
        {
            return HttpStatusCode.Forbidden;
        }

        return base.GetStatusCode(result);
    }
}
