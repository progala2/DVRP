using System;
using System.Collections.Generic;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.CommunicationServer.Components.Base;

namespace _15pl04.Ucc.CommunicationServer.Components
{
    public class SolverNodeInfo : ComponentInfo
    {
        public SolverNodeInfo(ComponentType type, ICollection<string> solvableProblems, byte numberOfThreads)
            : base(type, numberOfThreads)
        {
            if (type != ComponentType.ComputationalNode && type != ComponentType.TaskManager)
                throw new ArgumentException("Component type is neither Task Manager nor Computational Node.");

            SolvableProblems = solvableProblems;
        }

        public ICollection<string> SolvableProblems { get; private set; }
    }
}