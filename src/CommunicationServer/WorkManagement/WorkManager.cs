using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Utilities;
using _15pl04.Ucc.CommunicationServer.Collections;
using _15pl04.Ucc.CommunicationServer.Components;
using _15pl04.Ucc.CommunicationServer.Components.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement
{
    public class WorkManager : IWorkManager
    {
        public event WorkAssignmentEventHandler WorkAssignment;

        private IComponentOverseer _componentOverseer;

        private Random _random = new Random();
        private HashSet<ulong> _problemIdsInUse = new HashSet<ulong>();

        #region Problem & solution instance containers

        private LexicographicList<string, Problem> _problemsAwaitingDivision;                  // <ProblemType, P>
        private LexicographicList<ulong, Problem> _problemsBeingDivided;                       // <TaskManagerId, P>
        private Dictionary<ulong, Problem> _problemsAwaitingSolution;                          // <ProblemId, P>

        private LexicographicList<string, PartialProblem> _partialProblemsAwaitingComputation; // <ProblemType, PP>
        private LexicographicList<ulong, PartialProblem> _partialProblemsBeingComputed;        // <ComputationalNodeId, PP>

        private LexicographicList<ulong, PartialSolution> _partialSolutionsBeingGathered;      // <ProblemId, PS>
        private LexicographicList<string, PartialSolution[]> _partialSolutionsAwaitingMerge;   // <ProblemType, PS>
        private LexicographicList<ulong, PartialSolution[]> _partialSolutionsBeingMerged;      // <TaskManagerId, PS>

        private Dictionary<ulong, Solution> _finalSolutions;                                   // <ProblemId, S>

        #endregion

        public WorkManager(IComponentOverseer co)
        {
            _componentOverseer = co;
            _componentOverseer.Deregistration += OnComponentDeregistration;

            #region Containers' initialization

            _problemsAwaitingDivision = new LexicographicList<string, Problem>();
            _problemsBeingDivided = new LexicographicList<ulong, Problem>();
            _problemsAwaitingSolution = new Dictionary<ulong, Problem>();

            _partialProblemsAwaitingComputation = new LexicographicList<string, PartialProblem>();
            _partialProblemsBeingComputed = new LexicographicList<ulong, PartialProblem>();

            _partialSolutionsBeingGathered = new LexicographicList<ulong, PartialSolution>();
            _partialSolutionsAwaitingMerge = new LexicographicList<string, PartialSolution[]>();
            _partialSolutionsBeingMerged = new LexicographicList<ulong, PartialSolution[]>();

            _finalSolutions = new Dictionary<ulong, Solution>();

            #endregion


        }

        #region Getting work


        public bool TryGetWork(SolverNodeInfo node, out Work work)
        {
            if (!_componentOverseer.IsRegistered(node.ComponentId.Value))
                throw new ArgumentException("No registered component with the specified id is found.");

            switch(node.ComponentType)
            {
                case ComponentType.TaskManager:
                    foreach (string problemType in node.SolvableProblems)
                    {
                        PartialSolution[] solutions;
                        if (_partialSolutionsAwaitingMerge.TryRemoveFirst(problemType, out solutions))
                        {
                            work = new MergeWork(node.ComponentId.Value, solutions);
                            return true;
                        }

                        Problem problem;
                        if (_problemsAwaitingDivision.TryRemoveFirst(problemType, out problem))
                        {
                            ulong availableThreads = (ulong)CountAvailableSolvingThreads(problemType);
                            work = new DivisionWork(node.ComponentId.Value, problem, availableThreads);
                            return true;
                        }
                    }
                    break;

                case ComponentType.ComputationalNode:
                    foreach (string problemType in node.SolvableProblems)
                    {
                        // TODO: partial problem assignement can be optimized.

                        if (_partialProblemsAwaitingComputation.GetCountByKey(problemType) == 0)
                            continue;

                        int availableThreads = node.ThreadInfo
                            .Count(ti=>ti.State == ThreadStatus.ThreadState.Idle);

                        var partialProblems = new List<PartialProblem>(availableThreads);

                        for (int i = 0; i < availableThreads; i++)
                        {
                            PartialProblem pp;
                            if (_partialProblemsAwaitingComputation.TryRemoveFirst(problemType, out pp))
                                partialProblems.Add(pp);
                            else
                                break;
                        }

                        work = new ComputationWork(node.ComponentId.Value, partialProblems);
                        return true;
                    }
                    break;
            }

            work = null;
            return false;
        }

        public bool TryGetSolution(ulong problemId, out Solution solution)
        {
            if (!_finalSolutions.ContainsKey(problemId))
            {
                solution = null;
                return false;
            }
            else
            {
                solution = _finalSolutions[problemId];

                // Remove solution from the server.
                _finalSolutions.Remove(problemId);

                // Make freed problem id useable again.
                _problemIdsInUse.Remove(problemId);

                return true;
            }
        }


        #endregion


        #region Adding work


        public ulong AddProblem(Problem problem)
        {
            // Find unused id to assign.
            ulong id;
            do
            {
                id = _random.NextUInt64();
            } while (_problemIdsInUse.Contains(id));

            problem.Id = id; // Will throw an exception if Id is already set.
            _problemIdsInUse.Add(id);

            // Mark as "awaiting division".
            _problemsAwaitingDivision.AddLast(problem.ProblemType, problem);

            return id;
        }

        public void AddPartialProblems(IList<PartialProblem> partialProblems)
        {
            if (partialProblems == null)
                throw new ArgumentNullException();

            if (partialProblems.Count == 0)
                throw new ArgumentException();

            Problem problem = partialProblems[0].Problem;

            // Check if all partial problems are part of the same problem instance.
            foreach (PartialProblem pp in partialProblems)
                if (pp.Problem.Id != problem.Id)
                    throw new Exception("All partial problems must belong to the same problem instance.");

            // Check if there is a problem marked as "being divided". If so, proceed.
            if (_problemsBeingDivided.TryRemove(problem.Id.Value, problem))
            {
                _problemsAwaitingSolution.Add(problem.Id.Value, problem);
                _partialProblemsAwaitingComputation.AddLast(problem.ProblemType, partialProblems);
            }
        }

        public void AddSolution(Solution solution)
        {
            if (solution == null)
                throw new ArgumentNullException();

            ulong problemId = solution.Problem.Id.Value;

            // Check if there is a problem marked as "awaiting solution". If so, proceed. 
            if (_problemsAwaitingSolution.Remove(problemId))
                _finalSolutions.Add(problemId, solution);
        }

        public void AddPartialSolutions(IList<PartialSolution> partialSolutions)
        {
            if (partialSolutions == null)
                throw new ArgumentNullException();

            if (partialSolutions.Count == 0)
                throw new ArgumentException();

            Problem problem = partialSolutions[0].PartialProblem.Problem;

            // Check if all partial solutions are related to the same problem instance.
            foreach (PartialSolution ps in partialSolutions)
                if (ps.PartialProblem.Problem.Id != problem.Id)
                    throw new Exception("All partial solutions must be related to the same problem instance.");

            // Check if there is a problem marked as "awaiting solution". If so, proceed. 
            if (!_problemsAwaitingSolution.ContainsKey(problem.Id.Value))
                return; // TODO perhaps throw an exception?

            // Check if there are partial solutions related to the same problem instance that are already all gathered and awaiting merge.
            foreach (PartialSolution[] psArray in _partialSolutionsAwaitingMerge[problem.ProblemType])
            {
                if (psArray[0].PartialProblem.Problem.Id == problem.Id)
                    return; // TODO perhaps throw an exception?
            }

            // For each partial solution mark it as "being gathered" and  remove corresponding partial problem marked as "being computed".
            foreach (PartialSolution ps in partialSolutions)
            {
                if (_partialProblemsBeingComputed.TryRemove(ps.SolvingComputationalNodeId, ps.PartialProblem))
                    _partialSolutionsBeingGathered.AddLast(problem.Id.Value, ps);
                else
                    continue; // TODO perhaps throw an exception?
            }

            // Check if all partial solutions are in and we can merge them.
            ulong gatheredSolutions = (ulong)_partialSolutionsBeingGathered.GetCountByKey(problem.Id.Value);
            if (gatheredSolutions == problem.NumberOfParts.Value)
            {
                PartialSolution[] solutions = new PartialSolution[gatheredSolutions];
                _partialSolutionsBeingGathered.RemoveAllByKey(problem.Id.Value).CopyTo(solutions, 0);
                _partialSolutionsAwaitingMerge.AddLast(problem.ProblemType, solutions);
            }
        }


        #endregion

        private void OnComponentDeregistration(object sender, DeregisterationEventArgs e)
        {
            ulong id = e.Component.ComponentId.Value;

            switch (e.Component.ComponentType)
            {
                case ComponentType.TaskManager:
                    ICollection<Problem> problemsBeingDivided = _problemsBeingDivided.RemoveAllByKey(id);
                    if (problemsBeingDivided != null)
                    {
                        foreach (Problem p in problemsBeingDivided)
                        {
                            p.NumberOfParts = null;
                            _problemsAwaitingDivision.AddLast(p.ProblemType, p);
                        }
                    }

                    ICollection<PartialSolution[]> solutionsBeingMerged = _partialSolutionsBeingMerged.RemoveAllByKey(id);
                    if (solutionsBeingMerged != null)
                    {
                        foreach (PartialSolution[] psArray in solutionsBeingMerged)
                        {
                            string problemType = psArray[0].PartialProblem.Problem.ProblemType;
                            _partialSolutionsAwaitingMerge.AddLast(problemType, psArray);
                        }
                    }
                    break;

                case ComponentType.ComputationalNode:
                    ICollection<PartialProblem> problemsBeingComputed = _partialProblemsBeingComputed.RemoveAllByKey(id);
                    if (problemsBeingComputed != null)
                    {
                        foreach (PartialProblem pp in problemsBeingComputed)
                        {
                            string problemType = pp.Problem.ProblemType;
                            _partialProblemsAwaitingComputation.AddLast(problemType, pp);
                        }
                    }
                    break;

                case ComponentType.CommunicationServer:
                    BackupServerInfo backupInfo = e.Component as BackupServerInfo;

                    // TODO
                    break;
            }
        }

        private int CountAvailableSolvingThreads(string problemType)
        {
            int allThreads = _componentOverseer.GetComponents(ComponentType.ComputationalNode).Sum(c => c.NumberOfThreads);
            
            // TODO perhaps substract busy computational threads?
            // Not necessarily as it may equal zero.

            return allThreads;
        }
    }
}
