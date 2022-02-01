namespace MagicMedia.Processing;

public interface IMediaProcessorFlowFactory
{
    IMediaProcessorFlow CreateFlow(string name);
}
