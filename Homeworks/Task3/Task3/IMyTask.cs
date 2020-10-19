using System;

namespace Task3
{
    /// <summary>
    /// Represents the task that accepted for execution in the <see cref="MyThreadPool"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the calculation result.</typeparam>
    public interface IMyTask<TResult>
    {
        /// <summary>
        /// Gets a value that indicates whether the task has completed.
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Gets the calculation result. 
        /// If the result has not been calculated yet, blocks the calling thread until the task is completed. 
        /// </summary>
        /// <exception cref="AggregateException">Thrown if an exception was thrown during the execution of the task.
        /// The <see cref="AggregateException.InnerExceptions"/> contains the exception.</exception>
        TResult Result { get; }

        /// <summary>
        /// Creates a continuation task that executes after this task is completed.
        /// If the result has not been calculated yet, does not block the calling thread.
        /// </summary>
        /// <typeparam name="TNewResult">The type of the result produced by the continuation task.</typeparam>
        /// <param name="func">A function that can be applied to the result of this task.</param>
        /// <returns>The continuation task.</returns>
        IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> func);
    }
}
