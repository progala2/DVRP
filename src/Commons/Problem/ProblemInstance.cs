
namespace _15pl04.Ucc.Commons.Problem
{
    public class ProblemInstance
    {
        public ulong Id { get; private set; }
        public string Type { get; private set; }
        public byte[] Data { get; private set; }
        public ulong SolvingTimeout { get; private set; }

        public ProblemInstance(ulong id, string type, byte[] data, ulong timeout = 0)
        {
            Id = id;
            Type = type;
            Data = data;
            SolvingTimeout = timeout;
        }
    }
}
