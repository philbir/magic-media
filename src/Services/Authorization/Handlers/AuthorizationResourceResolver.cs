using System;
using System.Collections.Generic;
using System.Text.Json;
using HotChocolate;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;

namespace MagicMedia.Authorization
{
    public static class AuthorizationResourceResolver
    {
        public static ResourceInfo GetResourceInfo(object? resource)
        {
            var resourceInfo = new ResourceInfo();

            switch (resource)
            {
                case Guid resourceId:
                    resourceInfo.Source = "Parameter";
                    resourceInfo.Id = resourceId;
                    break;
                case IDirectiveContext gql:
                    resourceInfo.Source = "GraphQL";
                    resourceInfo.Raw = gql.Path.ToString();
                    resourceInfo.Type = MapFromRequest(gql);
                    if (gql.Variables.TryGetVariable("id", out Guid id))
                    {
                        resourceInfo.Id = id;
                    }
                    else if (gql.Variables.TryGetVariable("input", out object input))
                    {
                        resourceInfo.Id = input.GetType().GetProperty("Id")?.GetValue(input);
                        resourceInfo.Raw = JsonSerializer.Serialize(new GraphQLRequestInfo
                        {
                            Path = gql.Path.ToString(),
                            Input = input
                        });
                    }

                    break;
                case HttpContext http:
                    var routeId = http.GetRouteValue("id");
                    if (routeId != null)
                    {
                        resourceInfo.Source = "ApiController";
                        resourceInfo.Raw = http.Request.GetDisplayUrl();

                        if (Guid.TryParse(routeId.ToString(), out Guid parsed))
                        {
                            resourceInfo.Id = parsed;
                        }
                    }
                    break;
            }

            return resourceInfo;
        }

        private static ProtectedResourceType? MapFromRequest(IDirectiveContext gql)
        {
            foreach (var resType in Enum.GetValues(typeof(ProtectedResourceType)))
            {
                if ( gql.Path.ToString().Contains(resType.ToString(), StringComparison.InvariantCultureIgnoreCase))
                {
                    return (ProtectedResourceType) resType;
                }
            }

            return null;
        }

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

    public class GraphQLRequestInfo
    {
        public string Path { get; set; }

        public object Input { get; internal set; }
    }

}
