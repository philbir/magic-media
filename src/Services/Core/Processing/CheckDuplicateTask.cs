using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using MagicMedia.Configuration;
using Microsoft.Extensions.Logging;

namespace MagicMedia.Processing;

public class CheckDuplicateTask(
    ILogger<CheckDuplicateTask> logger,
    FileSystemStoreOptions options) : IMediaProcessorTask
{
    private readonly ILogger<CheckDuplicateTask> _logger = logger;
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
            logger.LogWarning("Duplicate Media, stop processing: {FileName}", context.File.Id);

            context.StopProcessing = true;

            var destination = Path.Combine(options.RootDirectory, "Duplicate");
            try
            {
                logger.LogInformation("Moving file to {Destination}", destination);
                File.Move(context.File.Id, Path.Combine(destination, Path.GetFileName(context.File.Id)), true);
            }
            catch (Exception ex)
            {
                logger.LogWarning("Could not move file: {FileName}: {Error}", context.File.Id, ex.Message);
            }
        }

        return Task.CompletedTask;
    }
}
