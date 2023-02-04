namespace MagicMedia.Processing;

public interface IMediaProcesserTaskFactory
{
    IMediaProcessorTask GetTask(string name);
}
