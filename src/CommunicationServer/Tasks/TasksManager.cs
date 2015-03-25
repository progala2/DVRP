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
        public delegate void FinalSolutionReceptionEventHandler(object sender, FinalSolutionReceptionEventArgs e);

        public event ProblemInstanceStateChangeEventHandler ProblemInstanceStateChange;
        public event PartialProblemStateChangeEventHandler PartialProblemStateChange;
        public event PartialSolutionStateChangeEventHandler PartialSolutionStateChange;
        public event FinalSolutionReceptionEventHandler FinalSolutionReception;

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static TasksManager Instance
        {
            get { return _lazy.Value; }
        }

        private static readonly Lazy<TasksManager> _lazy = new Lazy<TasksManager>(() => new TasksManager());


        private LexicographicQueue<string, ProblemInstance> _problemsAwaitingDivision;          // <ProblemType, PI>
        private LexicographicQueue<ulong, ProblemInstance> _problemsBeingDivided;               // <ProblemInstanceId, PI>
        private LexicographicQueue<ulong, ProblemInstance> _problemsAwaitingSolution;           // <ProblemInstanceId, PI>

        private LexicographicQueue<string, PartialProblem> _partialProblemsAwaitingComputation; // <ProblemType, PP>
        private LexicographicQueue<ulong, PartialProblem> _partialProblemsBeingComputed;        // <PartialProblemId, PP>

        private LexicographicQueue<ulong, PartialSolution> _partialSolutionsBeingGathered;      // <ProblemInstanceId, PS>
        private LexicographicQueue<string, PartialSolution[]> _partialSolutionsAwaitingMerge;   // <ProblemType, PS>
        private LexicographicQueue<ulong, PartialSolution[]> _partialSolutionsBeingMerged;      // <ProblemInstanceId, PS>

        private Dictionary<ulong, FinalSolution> _finalSolutions;                               // <ProblemInstanceId, FS>


        private TasksManager()
        {
            _problemsAwaitingDivision = new LexicographicQueue<string, ProblemInstance>();
            _problemsBeingDivided = new LexicographicQueue<ulong, ProblemInstance>();
            _problemsAwaitingSolution = new LexicographicQueue<ulong, ProblemInstance>();

            _partialProblemsAwaitingComputation = new LexicographicQueue<string, PartialProblem>();
            _partialProblemsBeingComputed = new LexicographicQueue<ulong, PartialProblem>();

            _partialSolutionsBeingGathered = new LexicographicQueue<ulong, PartialSolution>();
            _partialSolutionsAwaitingMerge = new LexicographicQueue<string, PartialSolution[]>();
            _partialSolutionsBeingMerged = new LexicographicQueue<ulong, PartialSolution[]>();

            _finalSolutions = new Dictionary<ulong, FinalSolution>();
        }

        /// <summary>
        /// Get a Problem Instance to divide if any is available. If so, mark it as "Being Divided".
        /// </summary>
        /// <param name="type">Type name od the problem.</param>
        /// <param name="problemInstance">Returned Problem Instance.</param>
        /// <param name="taskManagerId">Task Manager assigned to make the division.</param>
        /// <returns>True if any Problem Instance returned. False otherwise.</returns>
        public bool GetProblemInstanceToDivide(string type, out ProblemInstance problemInstance, ulong taskManagerId)
        {
            if (_problemsAwaitingDivision.TryDequeue(type, out problemInstance))
            {
                int availableThreads = ComponentMonitor.Instance.GetNumberOfAvailableComputationalThreads(type);
                /*
                 * TODO: how is the number of available threads changing right now?
                 */
                problemInstance.NumberOfParts = (ulong)availableThreads;
                problemInstance.DividingTaskManagerId = taskManagerId;
                _problemsBeingDivided.Enqueue(taskManagerId, problemInstance);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Add newly produced Partial Problems and mark them as "Awaiting Computation".
        /// </summary>
        /// <param name="partialProblems">Partial Problems to add.</param>
        public void AddPartialProblems(PartialProblem[] partialProblems)
        {
            ProblemInstance problemInstance;
            if (_problemsBeingDivided.TryDequeue(partialProblems[0].ProblemInstanceId, out problemInstance))
            {
                problemInstance.NumberOfParts = (ulong)partialProblems.Length; // Perhaps check if those two are equal and throw an exception?
                problemInstance.DividingTaskManagerId = null;
                _problemsAwaitingSolution.Enqueue(problemInstance.Id, problemInstance);

                foreach (var p in partialProblems)
                    _partialProblemsAwaitingComputation.Enqueue(p.ProblemType, p);
            }
        }

        /// <summary>
        /// Get one or more Partial Problems in order to send them to a Computational Node. Mark them as "Being Computed".
        /// </summary>
        /// <param name="type">Type name od the problem.</param>
        /// <param name="max">Maximum number of Partial Problems the Computational Node will take.</param>
        /// <param name="partialProblems">Returned Partial Problems.</param>
        /// <param name="compNodeId">Computational Node assigned to compute the Partial Problems.</param>
        /// <returns>True if at least one Partial Problem was returned. False otherwise.</returns>
        public bool GetPartialProblemsToCompute(string type, int max, out PartialProblem[] partialProblems, ulong compNodeId)
        {
            var output = new List<PartialProblem>(max);
            for (int i = 0; i < max; ++i)
            {
                PartialProblem pp;
                if (!_partialProblemsAwaitingComputation.TryDequeue(type, out pp))
                    break;

                pp.SolvingComputationalNodeId = compNodeId;
                output.Add(pp);
                _partialProblemsBeingComputed.Enqueue(p.PartialProblemId, pp);
            }

            if (output.Count > 0)
            {
                partialProblems = output.ToArray();
                return true;
            }
            else
            {
                partialProblems = null;
                return false;
            }
        }

        /// <summary>
        /// Add newly computed Partial Solutions, check if all of them are in.
        /// </summary>
        /// <param name="partialSolutions">Partial Solutions to add.</param>
        public void AddPartialSolutions(PartialSolution[] partialSolutions)
        {
            foreach (var ps in partialSolutions)
            {
                PartialProblem pp;
                if (_partialProblemsBeingComputed.TryDequeue(ps.PartialProblemId, out pp))
                {
                    pp.SolvingComputationalNodeId = null;
                    _partialSolutionsBeingGathered.Enqueue(ps.ProblemInstanceId, ps);
                }
            }
            // TODO: check if all solutions are in
        }

        public bool GetPartialSolutionsToMerge(string type, out PartialSolution[] partialSolutions, ulong taskManagerId)
        {
            if (_partialSolutionsAwaitingMerge.TryDequeue(type, out partialSolutions))
            {
                foreach (var ps in partialSolutions)
                    ps.MergingTaskManagerId = taskManagerId;

                _partialSolutionsBeingMerged.Enqueue(partialSolutions[0].ProblemInstanceId, partialSolutions);
                return true;
            }
            else
                return false;
        }

        public void AddFinalSolution(FinalSolution finalSolution)
        {
            ProblemInstance pi;
            if (_problemsAwaitingSolution.TryDequeue(finalSolution.ProblemInstanceId, out pi))
                _finalSolutions.Add(finalSolution.ProblemInstanceId, finalSolution);
        }


        /*
         * TODO - what if processing components are deregistered?
         * TODO - what is computation timeout occurs?
         * TODO - raise events.
         */
    }
}
