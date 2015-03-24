using _15pl04.Ucc.Commons;
using System;

namespace _15pl04.Ucc.CommunicationServer
{
    /// <summary>
    /// Class that represents a registred component in Communication Server.
    /// </summary>
    public class ComponentInfo
    {
        /// <summary>
        /// Component type.
        /// </summary>
        public ComponentType Type { get; private set; }
        /// <summary>
        /// Number of threads provided by the component.
        /// </summary>
        public byte NumberOfThreads { get; private set; }
        /// <summary>
        /// Names of computational problems solvable/supported by the component.
        /// </summary>
        public string[] SolvableProblems { get; private set; }
        /// <summary>
        /// Timestamp of the last connection between the component and the Computational Server.
        /// </summary>
        public DateTime Timestamp { get; private set; } // DateTime class might not be the best time measuring tool.
        /// <summary>
        /// Number of milliseconds between now and time of the timestamp.
        /// </summary>
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

        /// <summary>
        /// Updates the timestamp so it represents the current time.
        /// </summary>
        public void UpdateTimestamp()
        {
            Timestamp = DateTime.UtcNow;
        }
    }
}
