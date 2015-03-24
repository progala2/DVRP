using _15pl04.Ucc.Commons;
using System;
using System.Collections.Generic;

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
        public int NumberOfThreads 
        { 
            get { return _numberOfThreads; } 
        }
        /// <summary>
        /// Number of currently idle threads within the component.
        /// </summary>
        public int NumberOfIdleThreads 
        {
            get { return _numberOfIdleThreads; } 
            set { _numberOfIdleThreads = value; } 
        }
        /// <summary>
        /// Number of currently busy threads within the component.
        /// </summary>
        public int NumberOfBusyThreads 
        {
            get { return _numberOfThreads - _numberOfIdleThreads; }
            set { _numberOfIdleThreads = _numberOfThreads - value; }
        }
        /// <summary>
        /// Names of computational problems solvable/supported by the component.
        /// </summary>
        public HashSet<string> SolvableProblems 
        { 
            get { return _solvableProblems; } 
        }
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

        private readonly int _numberOfThreads;
        private int _numberOfIdleThreads;
        private readonly HashSet<string> _solvableProblems;

        public ComponentInfo(ComponentType type, byte numberOfThreads, string[] solvableProblems)
        {
            Type = type;
            _numberOfThreads = numberOfThreads;
            _solvableProblems = new HashSet<string>(solvableProblems);

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
