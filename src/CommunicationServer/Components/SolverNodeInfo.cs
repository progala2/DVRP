using System;
using System.Collections.Generic;
using Dvrp.Ucc.Commons.Components;
using Dvrp.Ucc.CommunicationServer.Components.Base;

namespace Dvrp.Ucc.CommunicationServer.Components
{
    /// <summary>
    /// Information about Task Manager/Computational Node component.
    /// </summary>
    public class SolverNodeInfo : ComponentInfo
    {
	    /// <summary>
	    /// Creates SolverNodeInfo instance.
	    /// </summary>
	    /// <param name="id"></param>
	    /// <param name="type">Type of the component.</param>
	    /// <param name="solvableProblems">Collection of solvable problem types.</param>
	    /// <param name="numberOfThreads">Number of threads provided by the component.</param>
	    public SolverNodeInfo(ulong id, ComponentType type, ICollection<string> solvableProblems, byte numberOfThreads)
            : base(id, type, numberOfThreads)
        {
            if (type != ComponentType.ComputationalNode && type != ComponentType.TaskManager)
                throw new ArgumentException("Component type is neither Task Manager nor Computational Node.");

            SolvableProblems = solvableProblems;
        }

        /// <summary>
        /// Collection of names of the problem types the component can process.
        /// </summary>
        public ICollection<string> SolvableProblems { get; }
    }
}