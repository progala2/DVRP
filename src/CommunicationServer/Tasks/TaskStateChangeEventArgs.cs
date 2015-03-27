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
        public Tuple<PartialProblem, PartialProblemState>[] PartialProblems { get; set; }
    }

    internal class PartialSolutionStateChangeEventArgs : EventArgs
    {
        public Tuple<PartialSolution, PartialSolutionState>[] PartialSolutions { get; set; }
    }

    internal class FinalSolutionReceptionEventArgs : EventArgs
    {
        public FinalSolution FinalSolution { get; set; }
    }
}
