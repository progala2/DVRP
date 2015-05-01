using System;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons.Computations
{
    /// <summary>
    /// Represents a readonly status of thread in ComputationalNode or TaskManager.
    /// Provides information needed to create StatusMessage.
    /// </summary>
    public class ComputationalThreadStatus
    {
        /// <summary>
        /// Gets state of this thread.
        /// </summary>
        public ThreadStatus.ThreadState State { get { return _state; } }

        /// <summary>
        /// Gets date of the last state change.
        /// </summary>
        public DateTime LastStateChange { get { return _lastStateChange; } }

        /// <summary>
        /// Gets time since last state change.
        /// </summary>
        public TimeSpan TimeSinceLastStateChange { get { return DateTime.UtcNow - _lastStateChange; } }

        /// <summary>
        /// Gets the ID of the problem assigned when client connected which current computations refer to.
        /// </summary>
        public ulong? ProblemInstanceId { get { return _problemInstanceId; } }

        /// <summary>
        /// Gets the ID of the task within given problem instance which current computations refer to.
        /// </summary>
        public ulong? PartialProblemId { get { return _partialProblemId; } }

        /// <summary>
        /// Gets the name of the type as given by TaskSolver which current computations refer to.
        /// </summary>
        public string ProblemType { get { return _problemType; } }


        private readonly ThreadStatus.ThreadState _state;
        private readonly DateTime _lastStateChange;
        private readonly ulong? _problemInstanceId;
        private readonly ulong? _partialProblemId;
        private readonly string _problemType;


        /// <summary>
        /// Creates status of thread in idle state.
        /// </summary>
        public ComputationalThreadStatus()
        {
            _state = ThreadStatus.ThreadState.Idle;
            _lastStateChange = DateTime.UtcNow;

            _problemType = null;
            _problemInstanceId = null;
            _partialProblemId = null;
        }

        /// <summary>
        /// Creates status of thread in busy state.
        /// It also takes information to store about ongoing computation.
        /// </summary>
        /// <param name="problemType">The name of the type as given by TaskSolver.</param>
        /// <param name="problemInstanceId">The ID of the problem assigned when client connected.</param>
        /// <param name="partialProblemId">The ID of the task within given problem instance.</param>
        public ComputationalThreadStatus(string problemType, ulong? problemInstanceId, ulong? partialProblemId)
        {
            _state = ThreadStatus.ThreadState.Busy;
            _lastStateChange = DateTime.UtcNow;

            _problemType = problemType;
            _problemInstanceId = problemInstanceId;
            _partialProblemId = partialProblemId;
        }
    }
}
