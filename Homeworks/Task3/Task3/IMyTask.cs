using System;

namespace Task3
{
    public interface IMyTask<TResult>
    {
        bool IsCompleted { get; }

        TResult Result { get; }

        IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func);
    }
}
