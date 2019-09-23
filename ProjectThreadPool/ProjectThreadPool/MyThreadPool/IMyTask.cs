using System;

namespace MyThreadPool
{
    /// <summary>
    /// Интерфейс, который реализует ThreadPool.
    /// </summary>
    public interface IMyTask<TResult>
    {
        bool IsCompleted { get; }
        TResult Result { get; }
        IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> job);
    }
}
