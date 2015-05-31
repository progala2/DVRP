using System;
using System.Collections.Generic;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.CommunicationServer.Components.Base;

namespace _15pl04.Ucc.CommunicationServer.Components
{
    /// <summary>
    /// Information about Task Manager/Computational Node component.
    /// </summary>
    public class SolverNodeInfo : ComponentInfo
    {
        /// <summary>
        /// Creates SolverNodeInfo instance.
        /// </summary>
        /// <param name="type">Type of the component.</param>
        /// <param name="solvableProblems">Collection of solvable problem types.</param>
        /// <param name="numberOfThreads">Number of threads provided by the component.</param>
        public SolverNodeInfo(ComponentType type, ICollection<string> solvableProblems, byte numberOfThreads)
            : base(type, numberOfThreads)
        {
            if (type != ComponentType.ComputationalNode && type != ComponentType.TaskManager)
                throw new ArgumentException("Component type is neither Task Manager nor Computational Node.");

            SolvableProblems = solvableProblems;
        }

        /// <summary>
        /// Collection of names of the problem types the component can process.
        /// </summary>
        public ICollection<string> SolvableProblems { get; private set; }
    }
}