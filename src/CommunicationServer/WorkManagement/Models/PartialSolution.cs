namespace _15pl04.Ucc.CommunicationServer.WorkManagement.Models
{
    internal class PartialSolution
    {
        public enum PartialSolutionState
        {
            BeingGathered = 0,
            AwaitingMerge,
            BeingMerged
        }

        public PartialSolution(PartialProblem partialProblem, byte[] data, ulong computationsTime, bool timeoutOccured)
        {
            PartialProblem = partialProblem;
            Data = data;
            ComputationsTime = computationsTime;
            TimeoutOccured = timeoutOccured;
        }

        public PartialSolutionState State { get; set; }
        public ulong? MergingNodeId { get; set; }
        public PartialProblem PartialProblem { get; private set; }
        public byte[] Data { get; private set; }
        public ulong ComputationsTime { get; private set; }
        public bool TimeoutOccured { get; private set; }
    }
}