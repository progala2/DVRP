namespace _15pl04.Ucc.CommunicationServer.WorkManagement.Models
{
    /// <summary>
    /// Contains data and metadata concerning a partial solution.
    /// </summary>
    internal class PartialSolution
    {
        /// <summary>
        /// Possible states of a partial solution during its lifetime.
        /// </summary>
        public enum PartialSolutionState
        {
            BeingGathered = 0,
            AwaitingMerge,
            BeingMerged
        }

        /// <summary>
        /// </summary>
        /// <param name="partialProblem">Corresponding partial problem.</param>
        /// <param name="data">Solution data.</param>
        /// <param name="computationsTime">Foregoing computations time.</param>
        /// <param name="timeoutOccured">True if the computations that generated this partial solution were stopped due to the timeout.</param>
        public PartialSolution(PartialProblem partialProblem, byte[] data, ulong computationsTime, bool timeoutOccured)
        {
            PartialProblem = partialProblem;
            Data = data;
            ComputationsTime = computationsTime;
            TimeoutOccured = timeoutOccured;
        }
        /// <summary>
        /// Current state of the partial solution.
        /// </summary>
        public PartialSolutionState State { get; set; }
        /// <summary>
        /// ID of the task manager that is currently merging this partial solution. Null if isn't being merged.
        /// </summary>
        public ulong? MergingNodeId { get; set; }
        /// <summary>
        /// Corresponding partial problem.
        /// </summary>
        public PartialProblem PartialProblem { get; private set; }
        /// <summary>
        /// Partial solution data.
        /// </summary>
        public byte[] Data { get; private set; }
        /// <summary>
        /// Time of the foregoing computations.
        /// </summary>
        public ulong ComputationsTime { get; private set; }
        /// <summary>
        /// True if the computations that generated this partial solution were stopped due to the timeout.
        /// </summary>
        public bool TimeoutOccured { get; private set; }
    }
}