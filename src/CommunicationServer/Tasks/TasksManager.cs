using _15pl04.Ucc.CommunicationServer.Collections;
using _15pl04.Ucc.CommunicationServer.Tasks.Models;
using System;
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
        private LexicographicQueue<uint, ProblemInstance> _problemsBeingComputed;

        private LexicographicQueue<string, PartialProblem> _partialProblemsToCompute;
        private LexicographicQueue<ulong, PartialProblem> _partialProblemsBeingComputed;

        private LexicographicQueue<ulong, PartialSolution> _partialSolutionsBeingGathered;
        private LexicographicQueue<string, PartialSolution[]> _partialSolutionsToMerge;
        private LexicographicQueue<string, PartialSolution[]> _partialSolutionsBeingMerged;

        private Dictionary<ulong, FinalSolution> _finalSolutions;


        private TasksManager()
        {
            _problemsToDivide = new LexicographicQueue<string, ProblemInstance>();
            _problemsBeingDivided = new LexicographicQueue<string, ProblemInstance>();
            _problemsBeingComputed = new LexicographicQueue<uint,ProblemInstance>();

            _partialProblemsToCompute = new LexicographicQueue<string, PartialProblem>();
            _partialProblemsBeingComputed = new LexicographicQueue<ulong, PartialProblem>();

            _partialSolutionsBeingGathered = new LexicographicQueue<ulong, PartialSolution>();
            _partialSolutionsToMerge = new LexicographicQueue<string, PartialSolution[]>();
            _partialSolutionsBeingMerged = new LexicographicQueue<string, PartialSolution[]>();

            _finalSolutions = new Dictionary<ulong, FinalSolution>();
        }

        

    }
}
