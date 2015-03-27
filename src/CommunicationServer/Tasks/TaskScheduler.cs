using _15pl04.Ucc.CommunicationServer.Collections;
using _15pl04.Ucc.CommunicationServer.Tasks.Models;
using System;
using System.Collections.Generic;

namespace _15pl04.Ucc.CommunicationServer.Tasks
{
    internal sealed class TaskScheduler
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
        public static TaskScheduler Instance
        {
            get { return _lazy.Value; }
        }

        private static readonly Lazy<TaskScheduler> _lazy = new Lazy<TaskScheduler>(() => new TaskScheduler());


        private LexicographicQueue<string, ProblemInstance> _problemsAwaitingDivision;          // <ProblemType, PI>
        private LexicographicQueue<ulong, ProblemInstance> _problemsBeingDivided;               // <TaskManagerId, PI>
        private Dictionary<ulong, ProblemInstance> _problemsAwaitingSolution;                   // <ProblemInstanceId, PI>

        private LexicographicQueue<string, PartialProblem> _partialProblemsAwaitingComputation; // <ProblemType, PP>
        private LexicographicQueue<ulong, PartialProblem> _partialProblemsBeingComputed;        // <ComputationalNodeId, PP>

        private LexicographicQueue<ulong, PartialSolution> _partialSolutionsBeingGathered;      // <ProblemInstanceId, PS>
        private LexicographicQueue<string, PartialSolution[]> _partialSolutionsAwaitingMerge;   // <ProblemType, PS>
        private LexicographicQueue<ulong, PartialSolution[]> _partialSolutionsBeingMerged;      // <TaskManagerId, PS>

        private Dictionary<ulong, FinalSolution> _finalSolutions;                               // <ProblemInstanceId, FS>


        private TaskScheduler()
        {
            _problemsAwaitingDivision = new LexicographicQueue<string, ProblemInstance>();
            _problemsBeingDivided = new LexicographicQueue<ulong, ProblemInstance>();
            _problemsAwaitingSolution = new Dictionary<ulong, ProblemInstance>();

            _partialProblemsAwaitingComputation = new LexicographicQueue<string, PartialProblem>();
            _partialProblemsBeingComputed = new LexicographicQueue<ulong, PartialProblem>();

            _partialSolutionsBeingGathered = new LexicographicQueue<ulong, PartialSolution>();
            _partialSolutionsAwaitingMerge = new LexicographicQueue<string, PartialSolution[]>();
            _partialSolutionsBeingMerged = new LexicographicQueue<ulong, PartialSolution[]>();

            _finalSolutions = new Dictionary<ulong, FinalSolution>();


            ComponentMonitor.Instance.Deregistration += OnComponentDeregistration;
        }

        public void AddNewProblemInstance(ProblemInstance problemInstance)
        {
            _problemsAwaitingDivision.Enqueue(problemInstance.Type, problemInstance);
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
                _problemsAwaitingSolution.Add(problemInstance.Id, problemInstance);

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
                _partialProblemsBeingComputed.Enqueue(compNodeId, pp);
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
            var keys = new HashSet<ulong>();

            foreach (var ps in partialSolutions)
            {
                PartialProblem pp;
                if (_partialProblemsBeingComputed.TryDequeue(ps.PartialProblemId, out pp))
                {
                    pp.SolvingComputationalNodeId = null;
                    _partialSolutionsBeingGathered.Enqueue(ps.ProblemInstanceId, ps);
                    keys.Add(ps.ProblemInstanceId);
                }
            }

            // Check if all solutions are in.
            foreach (var k in keys)
            {
                if ((ulong)_partialSolutionsBeingGathered.GetCount(k) == _problemsAwaitingSolution[k].NumberOfParts)
                {
                    var solutions = _partialSolutionsBeingGathered.RemoveAllByKey(k).ToArray();
                    var problemType = solutions[0].ProblemType;
                    _partialSolutionsAwaitingMerge.Enqueue(problemType, solutions);
                }
            }
        }

        /// <summary>
        /// Get all Partial Solutions produced from a single Problem Instance. Mark them as "Being Merged".
        /// </summary>
        /// <param name="type">Type name od the problem.</param>
        /// <param name="partialSolutions">Returned set of Partial Solutions to merge.</param>
        /// <param name="taskManagerId">Task Manager assigned to make the merge.</param>
        /// <returns>True if Partial Solutions are returned. False if there are no Partial Solutions of specified type that need merging.</returns>
        public bool GetPartialSolutionsToMerge(string type, out PartialSolution[] partialSolutions, ulong taskManagerId)
        {
            if (_partialSolutionsAwaitingMerge.TryDequeue(type, out partialSolutions))
            {
                foreach (var ps in partialSolutions)
                    ps.MergingTaskManagerId = taskManagerId;

                _partialSolutionsBeingMerged.Enqueue(taskManagerId, partialSolutions);
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Adds newly computed Final Solution and make it available to the Computational Client.
        /// </summary>
        /// <param name="finalSolution"></param>
        public void AddFinalSolution(FinalSolution finalSolution)
        {
            _finalSolutions.Add(finalSolution.ProblemInstanceId, finalSolution);
            if (!_problemsAwaitingSolution.Remove(finalSolution.ProblemInstanceId))
                throw new Exception("Received final solution for a problem that doesn't expect one.");
        }

        public bool TryGetFinalSolution(ulong id, out FinalSolution finalSolution)
        {
            return _finalSolutions.TryGetValue(id, out finalSolution);
        }

        /// <summary>
        /// Makes all ongoing tasks of deregistered components available to process again.
        /// </summary>
        /// <param name="sender">Handler caller.</param>
        /// <param name="e">Information about the component.</param>
        private void OnComponentDeregistration(object sender, DeregisterationEventArgs e)
        {
            // TODO - check if this method is thread safe

            var undividedProblemInstances = _problemsBeingDivided.RemoveAllByKey(e.Id);
            if (undividedProblemInstances != null)
            {
                var problemType = undividedProblemInstances.Peek().Type;
                _problemsAwaitingDivision.Enqueue(problemType, undividedProblemInstances);
            }

            var uncomputedPartialProblems = _partialProblemsBeingComputed.RemoveAllByKey(e.Id);
            if (uncomputedPartialProblems != null)
            {
                var problemType = uncomputedPartialProblems.Peek().ProblemType;
                _partialProblemsAwaitingComputation.Enqueue(problemType, uncomputedPartialProblems);
            }

            var unmergedPartialSolutions = _partialSolutionsBeingMerged.RemoveAllByKey(e.Id);
            if (unmergedPartialSolutions != null)
            {
                var problemType = unmergedPartialSolutions.Peek()[0].ProblemType;
                _partialSolutionsAwaitingMerge.Enqueue(problemType, unmergedPartialSolutions);
            }
        }

        public ulong GenerateProblemInstanceId()
        {
            return 0; // TODO
        }

        /*
         * TODO - what is computation timeout occurs?
         * TODO - raise events.
         * TODO - add comments.
         */
    }
}
