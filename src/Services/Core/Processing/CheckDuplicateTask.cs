using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Configuration;
using MagicMedia.Discovery;
using Microsoft.Extensions.Logging;

namespace MagicMedia.Processing;

public class CheckDuplicateTask(ILogger<CheckDuplicateTask> logger, FileSystemStoreOptions storeOptions) : IMediaProcessorTask
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
            logger.DuplicateMediaFound(context.File.Id);

            context.StopProcessing = true;

            var duplicateDestination = Path.Combine(storeOptions.RootDirectory, "Duplicate");
            string moveTo = Path.Combine(duplicateDestination, Path.GetFileName(context.File.Id));

            try
            {
                logger.MoveDuplicateFile(context.File.Id, moveTo);
                File.Move(context.File.Id, moveTo, true);
            }
            catch (Exception ex)
            {
                logger.CouldNotMoveFile(moveTo, ex);
            }
        }

        return Task.CompletedTask;
    }
}
