using _15pl04.Ucc.Commons;
using _15pl04.Ucc.CommunicationServer.Components.Base;
using System;
using System.Collections.Generic;

namespace _15pl04.Ucc.CommunicationServer.Components
{
    public class SolvingNodeInfo : ComponentInfo
    {
        public ICollection<string> SolvableProblems { get; private set; }

        public SolvingNodeInfo(ComponentType type, ICollection<string> solvableProblems, byte numberOfThreads)
            : base(type, numberOfThreads)
        {
            if (type != ComponentType.ComputationalNode && type != ComponentType.TaskManager)
                throw new ArgumentException("Component type is neither Task Manager nor Computational Node.");

            SolvableProblems = solvableProblems;
        }
    }
}
