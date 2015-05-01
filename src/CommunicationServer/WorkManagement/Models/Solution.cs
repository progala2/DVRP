namespace _15pl04.Ucc.CommunicationServer.WorkManagement.Models
{
    internal class Solution
    {
        public Solution(Problem problem, byte[] data, ulong computationsTime, bool timeoutOccured)
        {
            Problem = problem;
            Data = data;
            ComputationsTime = computationsTime;
            TimeoutOccured = timeoutOccured;
        }

        public Problem Problem { get; private set; }
        public byte[] Data { get; private set; }
        public ulong ComputationsTime { get; private set; }
        public bool TimeoutOccured { get; private set; }
    }
}