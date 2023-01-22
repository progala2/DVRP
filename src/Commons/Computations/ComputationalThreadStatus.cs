using System;
using _15pl04.Ucc.Commons.Components;

namespace _15pl04.Ucc.Commons.Computations
{
    /// <summary>
    ///     Represents a readonly status of thread in ComputationalNode or TaskManager.
    ///     Provides information needed to create StatusMessage.
    /// </summary>
    public class ComputationalThreadStatus
    {
	    /// <summary>
        ///     Creates status of thread in idle state.
        /// </summary>
        public ComputationalThreadStatus()
        {
            State = ThreadStatus.ThreadState.Idle;
            LastStateChange = DateTime.UtcNow;

            ProblemType = null;
            ProblemInstanceId = null;
            PartialProblemId = null;
        }

        /// <summary>
        ///     Creates status of thread in busy state.
        ///     It also takes information to store about ongoing computation.
        /// </summary>
        /// <param name="problemType">The name of the type as given by TaskSolver.</param>
        /// <param name="problemInstanceId">The ID of the problem assigned when client connected.</param>
        /// <param name="partialProblemId">The ID of the task within given problem instance.</param>
        public ComputationalThreadStatus(string problemType, ulong? problemInstanceId, ulong? partialProblemId)
        {
            State = ThreadStatus.ThreadState.Busy;
            LastStateChange = DateTime.UtcNow;

            ProblemType = problemType;
            ProblemInstanceId = problemInstanceId;
            PartialProblemId = partialProblemId;
        }

        /// <summary>
        ///     Gets state of this thread.
        /// </summary>
        public ThreadStatus.ThreadState State { get; }

        /// <summary>
        ///     Gets date of the last state change.
        /// </summary>
        public DateTime LastStateChange { get; }

        /// <summary>
        ///     Gets time since last state change.
        /// </summary>
        public TimeSpan TimeSinceLastStateChange => DateTime.UtcNow - LastStateChange;

        /// <summary>
        ///     Gets the ID of the problem assigned when client connected which current computations refer to.
        /// </summary>
        public ulong? ProblemInstanceId { get; }

        /// <summary>
        ///     Gets the ID of the task within given problem instance which current computations refer to.
        /// </summary>
        public ulong? PartialProblemId { get; }

        /// <summary>
        ///     Gets the name of the type as given by TaskSolver which current computations refer to.
        /// </summary>
        public string? ProblemType { get; }
    }
}