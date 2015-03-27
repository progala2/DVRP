using _15pl04.Ucc.Commons;
using System;
using System.Collections.Generic;

namespace _15pl04.Ucc.CommunicationServer
{
    /// <summary>
    /// Class that represents a registred component in Communication Server.
    /// </summary>
    public class NodeInfo : ComponentInfo
    {
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
        public int NumberOfIdleThreads // TODO - change this value accordingly
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

        private readonly int _numberOfThreads;
        private int _numberOfIdleThreads;
        private readonly HashSet<string> _solvableProblems;

        public NodeInfo(ComponentType type, byte numberOfThreads, ICollection<string> solvableProblems)
            : base(type)
        {
            _numberOfThreads = numberOfThreads;
            _solvableProblems = new HashSet<string>(solvableProblems);
        }
    }
}
