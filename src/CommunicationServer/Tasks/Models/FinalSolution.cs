
namespace _15pl04.Ucc.CommunicationServer.Tasks.Models
{
    internal class FinalSolution
    {
        public byte[] SolutionData { get; private set; }
        public ulong ComputationsTime { get; private set; }
        public bool TimeoutOccured { get; private set; }


        public ulong ProblemInstanceId { get { return _problemInstance.Id; } }
        public string ProblemType { get { return _problemInstance.Type; } }


        private readonly ProblemInstance _problemInstance;

        public FinalSolution(ProblemInstance problemInstance, byte[] solutionData, ulong computationsTime, bool timeoutOccured)
        {
            _problemInstance = problemInstance;

            SolutionData = solutionData;
            ComputationsTime = computationsTime;
            TimeoutOccured = timeoutOccured;
        }
    }
}
