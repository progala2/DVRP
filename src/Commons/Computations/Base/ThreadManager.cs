using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Dvrp.Ucc.Commons.Computations.Base
{
    /// <summary>
    ///     Represents a thread manager which provides starting actions in new threads.
    /// </summary>
    public abstract class ThreadManager
    {
        private readonly ConcurrentQueue<int> _availableThreads;
        private readonly ComputationalThreadStatus[] _computationalThreadStatuses;

        /// <summary>
        ///     Creates a thread manager which provides starting actions in new threads.
        /// </summary>
        /// <param name="parallelThreads">The maximum number of threads that could be efficiently run in parallel.</param>
        protected ThreadManager(byte parallelThreads)
        {
            ParallelThreads = parallelThreads;
            _computationalThreadStatuses = new ComputationalThreadStatus[parallelThreads];
            ThreadStatuses = Array.AsReadOnly(_computationalThreadStatuses);
            _availableThreads = new ConcurrentQueue<int>();
            for (var i = 0; i < _computationalThreadStatuses.Length; i++)
            {
                _computationalThreadStatuses[i] = new ComputationalThreadStatus();
                _availableThreads.Enqueue(i);
            }
        }

        /// <summary>
        ///     The maximum number of threads that could be efficiently run in parallel.
        /// </summary>
        public byte ParallelThreads { get; }

        /// <summary>
        ///     The information about thread statuses.
        /// </summary>
        public IReadOnlyCollection<ComputationalThreadStatus> ThreadStatuses { get; }

        /// <summary>
        ///     Starts executing action if there is an available idle thread. This method gets information needed for Status
        ///     messages.
        /// </summary>
        /// <param name="actionToExecute">An action to be executed in new thread. If null no new thread will be started.</param>
        /// <param name="actionOnException">
        ///     An action to be executed when exception occurs in started thread. If null exception
        ///     will be ignored.
        /// </param>
        /// <param name="problemType">The name of the type as given by TaskSolver.</param>
        /// <param name="problemInstanceId">The ID of the problem assigned when client connected.</param>
        /// <param name="partialProblemId">The ID of the task within given problem instance.</param>
        /// <returns>True if thread was successfully started; false otherwise.</returns>
        public bool StartInNewThread(Action actionToExecute, Action<Exception> actionOnException, string problemType,
            ulong? problemInstanceId, ulong? partialProblemId)
        {
            if (actionToExecute == null)
                return false;

            if (!_availableThreads.TryDequeue(out var threadIndex))
                return false;

            var started = StartInNewThread(() =>
            {
                _computationalThreadStatuses[threadIndex] = new ComputationalThreadStatus(problemType, problemInstanceId,
                    partialProblemId);
                try
                {
                    actionToExecute();
                }
                catch (Exception ex)
                {
                    actionOnException?.Invoke(ex);
                }
                finally
                {
                    _computationalThreadStatuses[threadIndex] = new ComputationalThreadStatus();
                    _availableThreads.Enqueue(threadIndex);
                }
            });

            return started;
        }

        /// <summary>
        ///     Tries to start executing action in new thread.
        /// </summary>
        /// <param name="actionToExecute">An action to be executed in new thread.</param>
        /// <returns>True if thread was successfully started; false otherwise.</returns>
        protected abstract bool StartInNewThread(Action actionToExecute);
    }
}