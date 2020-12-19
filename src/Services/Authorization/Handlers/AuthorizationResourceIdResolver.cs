using System;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace MagicMedia.Authorization
{
    public static class AuthorizationResourceIdResolver
    {
        public static Guid? TryGetId(object? resource)
        {
            Guid? id = null;

            switch (resource)
            {
                case Guid resourceId:
                    id = resourceId;
                    break;
                case IDirectiveContext gql:
                    id = gql.ArgumentValue<Guid>("id");
                    break;
                case HttpContext http:
                    var routeId = http.GetRouteValue("id");
                    if (routeId != null)
                    {
                        if (Guid.TryParse(routeId.ToString(), out Guid parsed))
                        {
                            id = parsed;
                        }
                    }
                    break;
            }

            return id;
        }
    }
}
