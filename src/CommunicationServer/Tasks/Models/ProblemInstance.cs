
namespace _15pl04.Ucc.CommunicationServer.Tasks.Models
{
    internal class ProblemInstance
    {
        public ulong Id { get; private set; }
        public string Type { get; private set; }
        public byte[] Data { get; private set; }
        public ulong SolvingTimeout { get; private set; }


        public ulong? DividingTaskManagerId { get; set; }
        public ulong NumberOfParts { get; set; }


        public ProblemInstance(ulong id, string type, byte[] data, ulong timeout = 0)
        {
            Id = id;
            Type = type;
            Data = data;
            SolvingTimeout = timeout;
        }
    }
}
