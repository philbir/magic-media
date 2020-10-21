using System;

namespace MagicMedia.Api.GraphQL.Face
{
    public class AssignPersonByHumanInput
    {
        public Guid FaceId { get; set; }
        public string PersonName { get; set; }
    }
}
