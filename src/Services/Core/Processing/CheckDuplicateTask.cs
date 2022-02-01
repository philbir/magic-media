using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace MagicMedia.Processing;

public class CheckDuplicateTask : IMediaProcessorTask
{
    public string Name => MediaProcessorTaskNames.CheckDuplicate;

    public Task ExecuteAsync(MediaProcessorContext context, CancellationToken cancellationToken)
    {

        var isDuplicate = context.Guard.IsDuplicate(context.Hashes);

        if (!isDuplicate)
        {
            context.Guard.AddMedia(context.Hashes);
        }

        else
        {
            //Log.Warning("Duplicate Media, stop processing");

            context.StopProcessing = true;

            var dest = @"D:\MagicMedia\Duplicate";
            try
            {
                File.Move(context.File.Id, Path.Combine(dest, Path.GetFileName(context.File.Id)), true);
            }
            catch (Exception ex)
            {
                //Log.Warning("Could not move file: {FileName}: {Error}", context.File.Id, ex.Message);
            }
        }

        return Task.CompletedTask;
    }
}
