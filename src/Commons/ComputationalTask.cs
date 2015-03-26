using System;
using System.Threading.Tasks;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons
{
    /// <summary>
    /// Represents a task running in ComputationalNode or TaskManager.
    /// Provides information needed to create StatusMessage.
    /// </summary>
    public class ComputationalTask
    {
        private StatusThreadState _state;
        private Task _task;


        public DateTime LastStateChange { get; private set; }

        public TimeSpan TimeSinceLastStateChange { get { return DateTime.UtcNow - LastStateChange; } }


        public StatusThreadState State
        {
            get { return _state; }
            private set
            {
                if (_state == value)
                    return;
                _state = value;
                LastStateChange = DateTime.UtcNow;
            }
        }

        private Task Task
        {
            get { return _task; }
            set
            {
                if (_task == value)
                    return;
                _task = value;
                State = _task == null ? StatusThreadState.Idle : StatusThreadState.Busy;
            }
        }


        public ulong? ProblemInstanceId { get; private set; }
        public ulong? PartialProblemId { get; private set; }
        public string ProblemType { get; private set; }


        /// <summary>
        /// Creates idle ComputationalTask which does nothing.
        /// </summary>
        public ComputationalTask()
            : this(null, null, null, null)
        { }
        
        /// <summary>
        /// Creates ComputationalTask with given <paramref name="task"/> to run and starts it immediately.
        /// </summary>
        /// <param name="task"></param>
        public ComputationalTask(Task task)
            : this(task, null, null, null)
        {        }

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

            Task = task;
            LastStateChange = DateTime.UtcNow;

            ProblemType = problemType;
            ProblemInstanceId = problemInstanceId;
            PartialProblemId = partialProblemId;

            task.Start();
        }
    }
}
