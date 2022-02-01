using System.Threading.Tasks;

namespace MagicMedia.Extensions;

public static class TaskExtensions
{
    public static void Forget(this Task task)
    {
        if (!task.IsCompleted || task.IsFaulted)
        {
            // https://docs.microsoft.com/en-us/dotnet/csharp/discards#a-standalone-discard
            _ = ForgetAwaited(task);
        }

        async static Task ForgetAwaited(Task task)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch
            {
                // Nothing to do here
            }
        }
    }
}
