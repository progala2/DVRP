
namespace _15pl04.Ucc.CommunicationServer.Tasks.Models
{
    internal class PartialSolution
    {
        public byte[] SolutionData { get; private set; }
        public ulong ComputationsTime { get; private set; }
        public bool TimeoutOccured { get; private set; }


        public ulong ProblemInstanceId { get { return _partialProblem.ProblemInstanceId; } }
        public ulong PartialProblemId { get { return _partialProblem.PartialProblemId; } }
        public string ProblemType { get { return _partialProblem.ProblemType; } }
        public byte[] CommonData { get { return _partialProblem.CommonData; } }


        private readonly PartialProblem _partialProblem;


        public PartialSolution(PartialProblem partialProblem, byte[] solutionData, ulong computationsTime, bool timeoutOccured)
        {
            _partialProblem = partialProblem;

            SolutionData = solutionData;
            ComputationsTime = computationsTime;
            TimeoutOccured = timeoutOccured;
        }
    }
}
