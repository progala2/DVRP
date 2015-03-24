using _15pl04.Ucc.Commons.Problem;
using _15pl04.Ucc.CommunicationServer.Collections;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace _15pl04.Ucc.CommunicationServer.Tasks
{
    internal sealed class TasksManager
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static TasksManager Instance
        {
            get { return _lazy.Value; }
        }

        private static readonly Lazy<TasksManager> _lazy = new Lazy<TasksManager>(() => new TasksManager());


        private LexicographicQueue<string, ProblemInstance> _problemsToDivide;
        private LexicographicQueue<string, ProblemInstance> _problemsBeingDivided;
        private LexicographicQueue<string, ProblemInstance> _problemsToMerge;
        private LexicographicQueue<string, ProblemInstance> _problemsBeingMerged;

        private LexicographicQueue<string, PartialProblem> _partialProblemsToCompute;
        private LexicographicQueue<string, PartialProblem> _partialProblemsBeingComputed;
        private LexicographicQueue<ulong, PartialProblem> _partialProblemsBeingGathered;
        private LexicographicQueue<string, PartialProblem[]> _partialProblemsToMerge;
        private LexicographicQueue<string, PartialProblem[]> _partialProblemsBeingMerged;

        private Dictionary<ulong, FinalSolution> _finalSolutions;


        private TasksManager()
        {
            _problemsToDivide = new LexicographicQueue<string, ProblemInstance>();
            _problemsBeingDivided = new LexicographicQueue<string, ProblemInstance>();
            _problemsToMerge = new LexicographicQueue<string, ProblemInstance>();
            _problemsBeingMerged = new LexicographicQueue<string, ProblemInstance>();

            _partialProblemsToCompute = new LexicographicQueue<string, PartialProblem>();
            _partialProblemsBeingComputed = new LexicographicQueue<string, PartialProblem>();
            _partialProblemsBeingGathered = new LexicographicQueue<ulong, PartialProblem>();
            _partialProblemsToMerge = new LexicographicQueue<string, PartialProblem[]>();
            _partialProblemsBeingMerged = new LexicographicQueue<string, PartialProblem[]>();

            _finalSolutions = new Dictionary<ulong, FinalSolution>();
        }

        
    }
}
