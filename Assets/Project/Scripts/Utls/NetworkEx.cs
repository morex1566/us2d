using System;
using System.Threading.Tasks;

public static class TaskExtensions
{
    public static async Task<bool> WaitAsyncEx(this Task task, int timeoutMs, Action onTimeout = null)
    {
        Task timeoutTask = Task.Delay(timeoutMs);

        if (await Task.WhenAny(task, timeoutTask) != task)
        {
            onTimeout?.Invoke();

            _ = task.ContinueWith(completedTask =>
            {
                _ = completedTask.Exception;
            },
            System.Threading.CancellationToken.None,
            TaskContinuationOptions.OnlyOnFaulted,
            TaskScheduler.Default);

            return false;
        }

        // task가 먼저 끝났더라도 실패했을 수 있으므로 예외를 꺼내기 위해 await 필요
        await task;
        return true;
    }
}
