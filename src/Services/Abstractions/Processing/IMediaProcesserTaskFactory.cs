namespace MagicMedia.Processing
{
    public interface IMediaProcesserTaskFactory
    {
        IMediaProcesserTask GetTask(string name);
    }
}