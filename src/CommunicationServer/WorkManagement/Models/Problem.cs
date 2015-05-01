namespace _15pl04.Ucc.CommunicationServer.WorkManagement.Models
{
    internal class Problem
    {
        public enum ProblemState
        {
            AwaitingDivision = 0,
            BeingDivided,
            AwaitingSolution
        }

        public Problem(ulong id, string type, byte[] data, ulong solvingTimeout)
        {
            Id = id;
            Type = type;
            Data = data;
            SolvingTimeout = solvingTimeout;
        }

        public ProblemState State { get; set; }
        public ulong? DividingNodeId { get; set; }
        public string Type { get; private set; }
        public ulong SolvingTimeout { get; private set; }
        public byte[] Data { get; private set; }
        public byte[] CommonData { get; set; }
        public ulong Id { get; private set; }
        public ulong? NumberOfParts { get; set; }
    }
}