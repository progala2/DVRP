#region Usings

using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Logging;
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

#endregion

namespace _15pl04.Ucc.CommunicationServer.WorkManagement
{
    internal class WorkManager : IWorkManager
    {

        #region Public fields


        public event WorkAssignmentEventHandler WorkAssignment;


        #endregion

        #region Private fields


        private static ILogger _logger = new TraceSourceLogger(typeof(WorkManager).Name);

        private Random _random = new Random();

        private IComponentOverseer _componentOverseer;


        private Dictionary<ulong, Problem> _problems;
        private Dictionary<ulong, Solution> _solutions;
        private Dictionary<Tuple<ulong, ulong>, PartialProblem> _partialProblems;
        private Dictionary<Tuple<ulong, ulong>, PartialSolution> _partialSolutions;


        #endregion

        #region Constructors

        public WorkManager(IComponentOverseer co)
        {
            _componentOverseer = co;
            _componentOverseer.Deregistration += OnComponentDeregistration;

            _problems = new Dictionary<ulong, Problem>();
            _solutions = new Dictionary<ulong, Solution>();
            _partialProblems = new Dictionary<Tuple<ulong, ulong>, PartialProblem>();
            _partialSolutions = new Dictionary<Tuple<ulong, ulong>, PartialSolution>();
        }

        #endregion

        #region Adding stuff


        public void AddProblem(string type, byte[] data, ulong solvingTimeout)
        {
            // Generate problem id.
            ulong id;
            do
            {
                id = _random.NextUInt64();
            } while (_problems.ContainsKey(id));

            // Create problem instance.
            Problem problem = new Problem(id, type, data, solvingTimeout);
            problem.State = Problem.ProblemState.AwaitingDivision;
            problem.DividingNodeId = null;

            // Add problem to the set.
            _problems.Add(id, problem);

            _logger.Info("Added new problem (id: " + id + ", type: " + type + ").");
        }

        public void AddPartialProblem(ulong problemId, ulong partialProblemId, byte[] privateData)
        {
            // Make sure the corresponding problem instance exists.
            if (!_problems.ContainsKey(problemId))
            {
                _logger.Error("Corresponding problem instance doesn't exist.");
                return;
            }

            Problem problem = _problems[problemId];

            // Make sure that state of the corresponding problem instance is set to "being divided".
            if (problem.State != Problem.ProblemState.BeingDivided)
            {
                _logger.Error("Corresponding problem's state is invalid.");
                return;
            }

            // Make sure the corresponding problem's property 'NumberOfParts' is set to a positive integer.
            if (!problem.NumberOfParts.HasValue || problem.NumberOfParts < 1)
            {
                _logger.Error("Corresponding problem's 'NumberOfParts' property is not a positive integer.");
                return;
            }

            // Make sure the problem instance's number of partial problems & partial solutions is not greater than 'NumberOfParts'.
            int ppNum = _partialProblems.Count(pair => pair.Key.Item1 == problemId);
            int psNum = _partialSolutions.Count(pair => pair.Key.Item1 == problemId);
            if (ppNum + psNum >= (int)problem.NumberOfParts)
            {
                _logger.Error("Received too many partial problems than expected.");
                return;
            }

            // Create partial problem instance.
            PartialProblem partialProblem = new PartialProblem(partialProblemId, problem, privateData);
            partialProblem.State = PartialProblem.PartialProblemState.AwaitingComputation;
            partialProblem.ComputingNodeId = null;

            // Add partial problem to the set.
            var pairId = new Tuple<ulong, ulong>(problemId, partialProblemId);
            _partialProblems.Add(pairId, partialProblem);

            _logger.Info("Added new partial problem (id: " + problemId + "/" + partialProblemId + ", type: " + problem.Type + ").");
        }

        public void AddSolution(ulong problemId, byte[] data, ulong computationsTime, bool timeoutOccured)
        {
            // Make sure the corresponding problem instance exists.
            if (!_problems.ContainsKey(problemId))
            {
                _logger.Error("Corresponding problem instance doesn't exist.");
                return;
            }

            Problem problem = _problems[problemId];

            // Make sure that state of the corresponding problem instance is set to "awaiting solution".
            if (problem.State != Problem.ProblemState.AwaitingSolution)
            {
                _logger.Error("Corresponding problem's state is invalid.");
                return;
            }

            // Make sure there are partial solutions in "being merged" state. Delete them.
            var mergedPartialSolutions = GetPartialSolutions(problemId, PartialSolution.PartialSolutionState.BeingMerged);
            foreach (PartialSolution ps in mergedPartialSolutions)
            {
                var pairId = new Tuple<ulong, ulong>(problemId, ps.PartialProblem.Id);
                _partialSolutions.Remove(pairId);
            }
            if (mergedPartialSolutions.Count == 0)
                _logger.Warn("There are no corresponding partial solutions in 'being merged' state.");

            // Create solution instance.
            Solution solution = new Solution(problem, data, computationsTime, timeoutOccured);

            // Add solution to the set.
            _solutions.Add(problemId, solution);

            // Delete corresponding problem.
            _problems.Remove(problemId);

            // Perform cleanup if any partial problem/solution related to the problem, is left.
            _partialProblems.RemoveAll((tuple, pp) =>
            {
                if (tuple.Item1 != problemId)
                    return false;

                _logger.Warn("Found a partial problem left out during the clean-up.");
                return true;
            });
            _partialSolutions.RemoveAll((tuple, ps) =>
            {
                if (tuple.Item1 != problemId)
                    return false;

                _logger.Warn("Found a partial solution left out during the clean-up.");
                return true;
            });

            _logger.Info("Added new problem (id: " + problemId + ", type: " + problem.Type + ").");
        }

        public void AddPartialSolution(ulong problemId, ulong partialProblemId, byte[] data, ulong computationsTime, bool timeoutOccured)
        {
            var pairId = new Tuple<ulong, ulong>(problemId, partialProblemId);

            // Make sure the corresponding problem instance exists.
            if (!_problems.ContainsKey(problemId))
            {
                _logger.Error("Corresponding problem instance doesn't exist.");
                return;
            }

            Problem problem = _problems[problemId];

            // Make sure the corresponding partial problem instance exists.
            if (!_partialProblems.ContainsKey(pairId))
            {
                _logger.Error("Corresponding partial problem doesn't exist.");
                return;
            }

            PartialProblem partialProblem = _partialProblems[pairId];

            // Make sure that state of the corresponding partial problem is set to "being computed".
            if (partialProblem.State != PartialProblem.PartialProblemState.BeingComputed)
            {
                _logger.Error("Corresponding partial problem's state is invalid.");
                return;
            }

            // Create partial solution instance.
            PartialSolution partialSolution = new PartialSolution(partialProblem, data, computationsTime, timeoutOccured);
            partialSolution.State = PartialSolution.PartialSolutionState.BeingGathered;
            partialSolution.MergingNodeId = null;

            // Add partial solution to the set.
            _partialSolutions.Add(pairId, partialSolution);

            // Delete corresponding partial problem.
            _partialProblems.Remove(pairId);

            _logger.Info("Added new partial solution (id: " + problemId + "/" + partialProblemId + ", type: " + problem.Type + ").");

            // Check if all partial solutions are in and set appropriate state.
            var gatheredPartialSolutions = GetPartialSolutions(problemId, PartialSolution.PartialSolutionState.BeingGathered);
            if ((int)problem.NumberOfParts == gatheredPartialSolutions.Count)
            {
                foreach (PartialSolution ps in gatheredPartialSolutions)
                    ps.State = PartialSolution.PartialSolutionState.AwaitingMerge;

                _logger.Info("Partial solutions are ready to merge (id: " + problemId + ").");
            }
        }


        #endregion

        #region Getting stuff


        public Solution GetSolution(ulong problemId)
        {
            return _solutions[problemId];
        }

        public Problem GetProblem(ulong problemId)
        {
            return _problems[problemId];
        }

        private ICollection<PartialProblem> GetPartialProblems(ulong problemId, PartialProblem.PartialProblemState? state = null)
        {
            return (ICollection<PartialProblem>)_partialProblems.Where(pair =>
                {
                    if (state.HasValue)
                        return problemId == pair.Key.Item1 && state == pair.Value.State;
                    else
                        return problemId == pair.Key.Item1;
                });
        }

        private ICollection<PartialSolution> GetPartialSolutions(ulong problemId, PartialSolution.PartialSolutionState? state = null)
        {
            return (ICollection<PartialSolution>)_partialSolutions.Where(pair =>
            {
                if (state.HasValue)
                    return problemId == pair.Key.Item1 && state == pair.Value.State;
                else
                    return problemId == pair.Key.Item1;
            });
        }


        #endregion

        #region Removing stuff


        public void RemoveSolution(ulong problemId)
        {
            _solutions.Remove(problemId);
            _logger.Info("Removed solution from the server (id: " + problemId + ").");
        }


        #endregion



        private void OnComponentDeregistration(object sender, DeregisterationEventArgs e)
        {
            ComponentType componentType = e.Component.ComponentType;

            if (componentType == ComponentType.CommunicationServer)
            {
                BackupServerInfo backup = e.Component as BackupServerInfo;

                throw new NotImplementedException();
                // TODO implement

            }
            else if (componentType == ComponentType.TaskManager)
            {
                SolverNodeInfo component = e.Component as SolverNodeInfo;

                var problemsBeingDivided = _problems.Values.Where(p =>
                    {
                        return p.DividingNodeId == component.ComponentId
                            && p.State == Problem.ProblemState.BeingDivided;
                    });

                foreach (Problem p in problemsBeingDivided)
                {
                    p.DividingNodeId = null;
                    p.State = Problem.ProblemState.BeingDivided;
                }

                var partialSolutionsBeingMerged = _partialSolutions.Values.Where(ps =>
                    {
                        return ps.MergingNodeId == component.ComponentId
                            && ps.State == PartialSolution.PartialSolutionState.BeingMerged;
                    });

                foreach (PartialSolution ps in partialSolutionsBeingMerged)
                {
                    ps.MergingNodeId = null;
                    ps.State = PartialSolution.PartialSolutionState.BeingMerged;
                }
            }
            else if (componentType == ComponentType.ComputationalNode)
            {
                SolverNodeInfo component = e.Component as SolverNodeInfo;

                var partialProblemsBeingComputed = _partialProblems.Values.Where(pp =>
                    {
                        return pp.ComputingNodeId == component.ComponentId
                            && pp.State == PartialProblem.PartialProblemState.BeingComputed;
                    });

                foreach (PartialProblem pp in partialProblemsBeingComputed)
                {
                    pp.ComputingNodeId = null;
                    pp.State = PartialProblem.PartialProblemState.AwaitingComputation;
                }
            }
        }

        private int CountAvailableSolvingThreads(string problemType)
        {
            int availableThreads = 0;
            
            foreach (SolverNodeInfo sn in _componentOverseer.GetComponents(ComponentType.ComputationalNode))
            {
                if (!sn.SolvableProblems.Contains(problemType))
                    continue;

                //availableThreads += sn.NumberOfThreads; // Count all threads.
                availableThreads += sn.ThreadInfo.Count(ts => ts.State == ThreadStatus.ThreadState.Idle); // OR count only the idle ones.
            }

            // No available threads found be we still need to divide the problem into some parts.
            if (availableThreads == 0)
                availableThreads = 3;

            return availableThreads;
        }


        public bool TryAssignWork(SolverNodeInfo node, out Work work)
        {
            ComponentType type = node.ComponentType;
            ulong nodeId = node.ComponentId.Value;

            if (type == ComponentType.TaskManager)
            {
                var partialSolutionsToMerge = _partialSolutions.Values.Where(ps =>
                    ps.State == PartialSolution.PartialSolutionState.AwaitingMerge
                    && node.SolvableProblems.Contains(ps.PartialProblem.Problem.Type))
                    as List<PartialSolution>;

                if (partialSolutionsToMerge.Count != 0)
                {
                    ulong problemId = partialSolutionsToMerge[0].PartialProblem.Problem.Id;

                    var solutionsToAssign = partialSolutionsToMerge.Where(ps =>
                        ps.PartialProblem.Problem.Id == problemId) 
                        as List<PartialSolution>;

                    foreach (PartialSolution ps in solutionsToAssign)
                    {
                        ps.State = PartialSolution.PartialSolutionState.BeingMerged;
                        ps.MergingNodeId = nodeId;
                    }

                    work = new MergeWork(nodeId, solutionsToAssign);

                    WorkAssignmentEventArgs e = new WorkAssignmentEventArgs()
                    {
                        AssigneeId = nodeId,
                        Work = work,
                    };
                    if (WorkAssignment != null)
                        WorkAssignment(this, e);

                    return true;
                }

                Problem problemToDivide = _problems.Values.FirstOrDefault(p =>
                    p.State == Problem.ProblemState.AwaitingDivision
                    && node.SolvableProblems.Contains(p.Type));

                if (problemToDivide != null)
                {
                    problemToDivide.State = Problem.ProblemState.BeingDivided;
                    problemToDivide.DividingNodeId = nodeId;
                    
                    int availableThreads = CountAvailableSolvingThreads(problemToDivide.Type);

                    work = new DivisionWork(nodeId, problemToDivide, (ulong)availableThreads);

                    WorkAssignmentEventArgs e = new WorkAssignmentEventArgs()
                    {
                        AssigneeId = nodeId,
                        Work = work,
                    };
                    if (WorkAssignment != null)
                        WorkAssignment(this, e);

                    return true;
                }

                work = null;
                return false;
            }
            else if (type == ComponentType.ComputationalNode)
            {
                int availableThreads = node.ThreadInfo.Count(ts => 
                    ts.State == ThreadStatus.ThreadState.Idle);

                var partialProblemsToCompute = _partialProblems.Values.Where(pp =>
                    pp.State == PartialProblem.PartialProblemState.AwaitingComputation
                    && node.SolvableProblems.Contains(pp.Problem.Type));

                var problemsToAssign = new List<PartialProblem>(availableThreads);

                foreach (PartialProblem pp in partialProblemsToCompute)
                {
                    if (availableThreads-- == 0)
                        break;

                    pp.State = PartialProblem.PartialProblemState.BeingComputed;
                    pp.ComputingNodeId = nodeId;

                    problemsToAssign.Add(pp);
                }

                if (problemsToAssign.Count != 0)
                {
                    work = new ComputationWork(nodeId, problemsToAssign);

                    WorkAssignmentEventArgs e = new WorkAssignmentEventArgs()
                    {
                        AssigneeId = nodeId,
                        Work = work,
                    };
                    if (WorkAssignment != null)
                        WorkAssignment(this, e);

                    return true;
                }

                work = null;
                return false;
            }
            else
                throw new InvalidOperationException();
        }

    }
}
