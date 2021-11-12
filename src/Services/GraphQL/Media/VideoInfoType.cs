using MagicMedia.Store;

namespace MagicMedia.GraphQL
{
    public partial class VideoInfoType : ObjectType<VideoInfo>
    {
        protected override void Configure(IObjectTypeDescriptor<VideoInfo> descriptor)
        {
            descriptor
                .Field("duration")
                .Argument("format", a => a
                    .DefaultValue(@"mm\:ss")
                    .Type(typeof(string)))
                .Type<StringType>()
                .Resolve(c =>
                {
                    VideoInfo? info = c.Parent<VideoInfo>();

                    return info?.Duration.ToString(c.Argument<string>("format"));
                });
        }
    }
}
