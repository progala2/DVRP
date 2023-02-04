namespace Dvrp.Ucc.CommunicationServer.WorkManagement.Models
{
    /// <summary>
    /// Contains data and metadata concerning a final solution.
    /// </summary>
    internal class Solution
    {
        /// <summary>
        /// Creates Solution instance.
        /// </summary>
        /// <param name="problem">Corresponding problem instance.</param>
        /// <param name="data">Final solution data.</param>
        /// <param name="computationsTime">Total computations time.</param>
        /// <param name="timeoutOccured">True if the computations were stopped due to timeout.</param>
        public Solution(Problem problem, byte[] data, ulong computationsTime, bool timeoutOccured)
        {
            Problem = problem;
            Data = data;
            ComputationsTime = computationsTime;
            TimeoutOccured = timeoutOccured;
        }

        /// <summary>
        /// Corresponding problem instance.
        /// </summary>
        public Problem Problem { get; }

        /// <summary>
        /// Final solution data.
        /// </summary>
        public byte[] Data { get; }

        /// <summary>
        /// Total computations time.
        /// </summary>
        public ulong ComputationsTime { get; }

        /// <summary>
        /// True if the computations were stopped due to timeout. False otherwise.
        /// </summary>
        public bool TimeoutOccured { get; }
    }
}