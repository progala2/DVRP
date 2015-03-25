using _15pl04.Ucc.CommunicationServer.Tasks.Models;
using System;

namespace _15pl04.Ucc.CommunicationServer.Tasks
{
    internal class ProblemInstanceStateChangeEventArgs : EventArgs
    {
        public ProblemInstanceState State { get; set; }
        public ProblemInstance ProblemInstance { get; set; }
    }

    internal class PartialProblemStateChangeEventArgs : EventArgs
    {
        public PartialProblemState State { get; set; }
        public PartialProblem PartialProblem { get; set; }
    }

    internal class PartialSolutionStateChangeEventArgs : EventArgs
    {
        public PartialSolutionState State { get; set; }
        public PartialSolution PartialSolution { get; set; }
    }

    internal class FinalSolutionReceptionEventArgs : EventArgs
    {
        public FinalSolution FinalSolution { get; set; }
    }
}
