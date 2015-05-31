using System;
using System.Threading;
using _15pl04.Ucc.Commons.Computations.Base;

namespace _15pl04.Ucc.Commons.Computations
{
    /// <summary>
    ///     Represents a thread manager which provides starting actions in new threads from thread pool.
    /// </summary>
    public sealed class ThreadPoolThreadManager : ThreadManager
    {
        /// <summary>
        ///     Creates a thread manager which provides starting actions in new threads from thread pool.
        /// </summary>
        public ThreadPoolThreadManager()
            : base((byte)Environment.ProcessorCount)
        {
        }

        /// <summary>
        ///     Tries to start executing action in new thread from thread pool.
        /// </summary>
        /// <param name="actionToExecute">An action to be executed in new thread.</param>
        /// <returns>True if thread was successfully started; false otherwise.</returns>
        protected override bool StartInNewThread(Action actionToExecute)
        {
            return ThreadPool.QueueUserWorkItem(arg => actionToExecute());
        }
    }
}