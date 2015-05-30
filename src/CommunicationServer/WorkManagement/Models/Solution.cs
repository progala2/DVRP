namespace _15pl04.Ucc.CommunicationServer.WorkManagement.Models
{
    /// <summary>
    /// Contains data and metadata concerning a final solution.
    /// </summary>
    internal class Solution
    {
        /// <summary>
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
        public Problem Problem { get; private set; }
        /// <summary>
        /// Final solution data.
        /// </summary>
        public byte[] Data { get; private set; }
        /// <summary>
        /// Total computations time.
        /// </summary>
        public ulong ComputationsTime { get; private set; }
        /// <summary>
        /// True if the computations were stopped due to timeout. False otherwise.
        /// </summary>
        public bool TimeoutOccured { get; private set; }
    }
}