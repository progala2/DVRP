using _15pl04.Ucc.Commons;
using System;

namespace _15pl04.Ucc.CommunicationServer
{
    public class ComponentInfo
    {
        public ComponentType Type { get; private set; }
        public byte NumberOfThreads { get; private set; }
        public string[] SolvableProblems { get; private set; }
        public DateTime Timestamp { get; private set; } // DateTime class might not be the best time measuring tool.
        public ulong TimestampAge
        {
            get
            {
                TimeSpan diff = DateTime.UtcNow - Timestamp;
                return (ulong)diff.TotalMilliseconds;
            }
        }

        public ComponentInfo(ComponentType type, byte numberOfThreads, string[] solvableProblems)
        {
            Type = type;
            NumberOfThreads = numberOfThreads;
            SolvableProblems = solvableProblems;

            Timestamp = DateTime.UtcNow;
        }

        public void UpdateTimestamp()
        {
            Timestamp = DateTime.UtcNow;
        }
    }
}
