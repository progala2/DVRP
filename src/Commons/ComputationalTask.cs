using System;
using System.Threading.Tasks;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons
{
    /// <summary>
    /// Represents a task in ComputationalNode or TaskManager.
    /// Provides information needed to create StatusMessage.
    /// </summary>
    public class ComputationalTask
    {
        private Task _task;


        public StatusThreadState State { get; private set; }

        public DateTime LastStateChange { get; private set; }

        public TimeSpan TimeSinceLastStateChange { get { return DateTime.UtcNow - LastStateChange; } }

        
        public ulong? ProblemInstanceId { get; private set; }

        public ulong? PartialProblemId { get; private set; }

        public string ProblemType { get; private set; }


        /// <summary>
        /// Creates idle ComputationalTask which does nothing.
        /// </summary>
        public ComputationalTask()
        {
            _task = null;
            State = StatusThreadState.Idle;
            LastStateChange = DateTime.UtcNow;
        }

        /// <summary>
        /// Creates ComputationalTask with given <paramref name="task"/> to run and starts it immediately.
        /// </summary>
        /// <param name="task"></param>
        public ComputationalTask(Task task)
            : this(task, null, null, null)
        { }

        /// <summary>
        /// Creates ComputationalTask with given <paramref name="task"/> to run and starts it immediately.
        /// It also takes information to store about starting computation.
        /// </summary>
        /// <param name="task">Task to start. It cannot be null.</param>
        /// <param name="problemType">The name of the type as given by TaskSolver.</param>
        /// <param name="problemInstanceId">The ID of the problem assigned when client connected.</param>
        /// <param name="partialProblemId">The ID of the task within given problem instance.</param>
        public ComputationalTask(Task task, string problemType, ulong? problemInstanceId, ulong? partialProblemId)
        {
            if (task == null)
                throw new ArgumentNullException("task");

            _task = task;
            State = StatusThreadState.Busy;
            LastStateChange = DateTime.UtcNow;

            ProblemType = problemType;
            ProblemInstanceId = problemInstanceId;
            PartialProblemId = partialProblemId;

            _task.Start();
        }
    }
}
