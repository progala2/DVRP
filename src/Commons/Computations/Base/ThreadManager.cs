using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace _15pl04.Ucc.Commons.Computations
{
    /// <summary>
    /// Represents a thread manager which provides starting and cancelling threads.
    /// </summary>
    public abstract class ThreadManager
    {
        /// <summary>
        /// The maximum number of threads that could be efficiently run in parallel.
        /// </summary>
        public byte ParallelThreads { get { return _parallelThreads; } }

        /// <summary>
        /// The information of thread statuses.
        /// </summary>
        public IReadOnlyCollection<ComputationalThreadStatus> ThreadStatuses { get { return _threadStatuses; } }


        private readonly byte _parallelThreads;
        private readonly ComputationalThreadStatus[] _computationalThreadStatuses;
        private readonly IReadOnlyCollection<ComputationalThreadStatus> _threadStatuses;
        private readonly ConcurrentQueue<int> _availableThreads;


        /// <summary>
        /// Creates a thread manager which provides starting threads.
        /// </summary>
        /// <param name="parallelThreads">The maximum number of threads that could be efficiently run in parallel.</param>
        protected ThreadManager(byte parallelThreads)
        {
            _parallelThreads = parallelThreads;
            _computationalThreadStatuses = new ComputationalThreadStatus[parallelThreads];
            _threadStatuses = Array.AsReadOnly(_computationalThreadStatuses);
            _availableThreads = new ConcurrentQueue<int>();
            for (int i = 0; i < _computationalThreadStatuses.Length; i++)
            {
                _computationalThreadStatuses[i] = new ComputationalThreadStatus();
                _availableThreads.Enqueue(i);
            }
        }


        /// <summary>
        /// Starts executing action if there is an available idle thread. This method gets information needed for Status messages.
        /// </summary>
        /// <param name="actionToExecute">An action to be executed.</param>
        /// <param name="problemType">The name of the type as given by TaskSolver.</param>
        /// <param name="problemInstanceId">The ID of the problem assigned when client connected.</param>
        /// <param name="partialProblemId">The ID of the task within given problem instance.</param>
        /// <returns>True if thread was successfully started; false otherwise.</returns>
        public bool StartInNewThread(Action actionToExecute, string problemType, ulong? problemInstanceId, ulong? partialProblemId)
        {
            int threadIndex;
            if (!_availableThreads.TryDequeue(out threadIndex))
                return false;

            bool started = StartInNewThread(() =>
            {
                _computationalThreadStatuses[threadIndex] = new ComputationalThreadStatus(problemType, problemInstanceId, partialProblemId);
                try
                {
                    actionToExecute();
                }
                finally
                {
                    _computationalThreadStatuses[threadIndex] = new ComputationalThreadStatus();
                    _availableThreads.Enqueue(threadIndex);
                }
            });

            return started;
        }


        protected abstract bool StartInNewThread(Action actionToExecute);
    }
}
