namespace MagicMedia.GraphQL.Face
{
    public partial class FaceMutations
    {
        public class BuildFaceModelPayload : Payload
        {
            public BuildFaceModelPayload(int faceCount)
            {
                FaceCount = faceCount;
            }

            public BuildFaceModelPayload(IReadOnlyList<UserError>? errors = null)
                : base(errors)
            {
            }

            public int FaceCount { get; }
        }
    }
}
