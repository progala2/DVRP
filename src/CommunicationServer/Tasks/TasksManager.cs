using _15pl04.Ucc.CommunicationServer.Collections;
using _15pl04.Ucc.CommunicationServer.Tasks.Models;
using System;
using System.Collections.Generic;

namespace _15pl04.Ucc.CommunicationServer.Tasks
{
    internal sealed class TasksManager
    {
        public delegate void ProblemInstanceStateChangeEventHandler(object sender, ProblemInstanceStateChangeEventArgs e);
        public delegate void PartialProblemStateChangeEventHandler(object sender, PartialProblemStateChangeEventArgs e);
        public delegate void PartialSolutionStateChangeEventHandler(object sender, PartialSolutionStateChangeEventArgs e);
        public delegate void FinalSolutionReception(object sender, FinalSolutionReceptionEventArgs e);

        public event ProblemInstanceStateChangeEventHandler ProblemInstanceStateChange;
        public event PartialProblemStateChangeEventHandler PartialProblemStateChange;
        public event PartialSolutionStateChangeEventHandler PartialSolutionStateChange;
        public event FinalSolutionReception FinalSolutionReception;

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static TasksManager Instance
        {
            get { return _lazy.Value; }
        }

        private static readonly Lazy<TasksManager> _lazy = new Lazy<TasksManager>(() => new TasksManager());


        private LexicographicQueue<string, ProblemInstance> _problemsAwaitingDivision;
        private LexicographicQueue<string, ProblemInstance> _problemsBeingDivided;
        private LexicographicQueue<uint, ProblemInstance> _problemsAwaitingSolution;

        private LexicographicQueue<string, PartialProblem> _partialProblemsAwaitingComputation;
        private LexicographicQueue<ulong, PartialProblem> _partialProblemsBeingComputed;

        private LexicographicQueue<ulong, PartialSolution> _partialSolutionsBeingGathered;
        private LexicographicQueue<string, PartialSolution[]> _partialSolutionsAwaitingMerge;
        private LexicographicQueue<string, PartialSolution[]> _partialSolutionsBeingMerged;

        private Dictionary<ulong, FinalSolution> _finalSolutions;


        private TasksManager()
        {
            _problemsAwaitingDivision = new LexicographicQueue<string, ProblemInstance>();
            _problemsBeingDivided = new LexicographicQueue<string, ProblemInstance>();
            _problemsAwaitingSolution = new LexicographicQueue<uint,ProblemInstance>();

            _partialProblemsAwaitingComputation = new LexicographicQueue<string, PartialProblem>();
            _partialProblemsBeingComputed = new LexicographicQueue<ulong, PartialProblem>();

            _partialSolutionsBeingGathered = new LexicographicQueue<ulong, PartialSolution>();
            _partialSolutionsAwaitingMerge = new LexicographicQueue<string, PartialSolution[]>();
            _partialSolutionsBeingMerged = new LexicographicQueue<string, PartialSolution[]>();

            _finalSolutions = new Dictionary<ulong, FinalSolution>();
        }

        

    }
}
