using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace _15pl04.Ucc.Commons
{
    public class ComputationalTaskPool
    {
        private ComputationalTask[] _computationalTasks;
        private ConcurrentQueue<int> _availableTasks;
        private CancellationToken _cancellationToken;

        public ReadOnlyCollection<ComputationalTask> ComputationalTasks { get; private set; }

        public ComputationalTaskPool(byte parallelThreads, CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;

            _computationalTasks = new ComputationalTask[parallelThreads];
            ComputationalTasks = Array.AsReadOnly(_computationalTasks);
            _availableTasks = new ConcurrentQueue<int>();

            for (int i = 0; i < _computationalTasks.Length; i++)
            {
                _computationalTasks[i] = new ComputationalTask();
                _availableTasks.Enqueue(i);
            }
        }


        /// <summary>
        /// Starts task if there is an available idle task in pool. This method gets information needed for Status messages.
        /// </summary>
        /// <param name="action">An action to be performed.</param>
        /// <param name="problemType">The name of the type as given by TaskSolver.</param>
        /// <param name="problemInstanceId">The ID of the problem assigned when client connected.</param>
        /// <param name="partialProblemId">The ID of the task within given problem instance.</param>
        /// <returns>Information whether task was successfully started.</returns>
        public bool StartComputationalTask(Action action, string problemType, ulong? problemInstanceId, ulong? partialProblemId)
        {
            // get index of idle task
            int taskIndex;
            if (!_availableTasks.TryDequeue(out taskIndex))
                return false;

            // create cancellable task
            var task = new Task(action, _cancellationToken);
            // after task completion reset proper ComputationalTask in array
            task.ContinueWith((t) =>
            {
                // set to new ComputationalTask with idle state
                _computationalTasks[taskIndex] = new ComputationalTask();
                _availableTasks.Enqueue(taskIndex);
            });

            // create ComputationalTask
            // it immediately starts given task
            _computationalTasks[taskIndex] = new ComputationalTask(task, problemType, problemInstanceId, partialProblemId);

            return true;
        }
    }
}
