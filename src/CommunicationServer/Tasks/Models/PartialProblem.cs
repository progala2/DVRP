
namespace _15pl04.Ucc.CommunicationServer.Tasks.Models
{
    internal class PartialProblem
    {
        public ulong PartialProblemId { get; private set; }
        public byte[] PrivateData { get; private set; }
        public byte[] CommonData { get; private set; }


        public ulong ProblemInstanceId { get { return _problemInstance.Id; } }
        public string ProblemType { get { return _problemInstance.Type; } }
        public ulong SolvingTimeout { get { return _problemInstance.SolvingTimeout; } }


        public ulong? SolvingComputationalNodeId { get; set; }


        private readonly ProblemInstance _problemInstance;

        public PartialProblem(ProblemInstance problemInstance, ulong id, byte[] privateData, byte[] commonData)
        {
            _problemInstance = problemInstance;

            PartialProblemId = id;
            PrivateData = privateData;
            CommonData = commonData;
        }
    }
}
