namespace MagicMedia.GraphQL
{
    public class SimilarMediaGroupType : ObjectType<SimilarMediaGroup>
    {
        private readonly IMediaService _mediaService;

        public SimilarMediaGroupType(IMediaService mediaService)
        {
            _mediaService = mediaService;
        }

        protected override void Configure(IObjectTypeDescriptor<SimilarMediaGroup> descriptor)
        {
            descriptor
                .Field("medias")
                .Resolve(x =>
                {
                    SimilarMediaGroup group = x.Parent<SimilarMediaGroup>();
                    return _mediaService.GetManyAsync(group.MediaIds, x.RequestAborted);
                });
        }
    }
}
