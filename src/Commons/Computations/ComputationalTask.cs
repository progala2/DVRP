using _15pl04.Ucc.Commons.Messaging.Models;
using System;
using System.Threading.Tasks;

namespace _15pl04.Ucc.Commons.Computations
{
    /// <summary>
    /// Represents a task in ComputationalNode or TaskManager.
    /// Provides information needed to create StatusMessage.
    /// </summary>
    public class ComputationalTask
    {
        /// <summary>
        /// Gets state of this task.
        /// </summary>
        public ThreadStatus.ThreadState State { get; private set; }

        /// <summary>
        /// Gets date of the last state change.
        /// </summary>
        public DateTime LastStateChange { get; private set; }

        /// <summary>
        /// Gets time since last state change.
        /// </summary>
        public TimeSpan TimeSinceLastStateChange { get { return DateTime.UtcNow - LastStateChange; } }

        /// <summary>
        /// Gets the ID of the problem assigned when client connected which current computations refer to.
        /// </summary>
        public ulong? ProblemInstanceId { get; private set; }

        /// <summary>
        /// Gets the ID of the task within given problem instance which current computations refer to.
        /// </summary>
        public ulong? PartialProblemId { get; private set; }

        /// <summary>
        /// Gets the name of the type as given by TaskSolver which current computations refer to.
        /// </summary>
        public string ProblemType { get; private set; }


        private Task _task;


        /// <summary>
        /// Creates idle ComputationalTask which does nothing.
        /// </summary>
        public ComputationalTask()
        {
            _task = null;
            State = ThreadStatus.ThreadState.Idle;
            LastStateChange = DateTime.UtcNow;
        }

        /// <summary>
        /// Creates ComputationalTask with given <paramref name="task"/> to run and starts it immediately.
        /// </summary>
        /// <param name="task"></param>
        public ComputationalTask(Task task)
            : this(task, null, null, null)
        {
        }

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
            State = ThreadStatus.ThreadState.Busy;
            LastStateChange = DateTime.UtcNow;

            ProblemType = problemType;
            ProblemInstanceId = problemInstanceId;
            PartialProblemId = partialProblemId;

            _task.Start();
        }
    }
}
