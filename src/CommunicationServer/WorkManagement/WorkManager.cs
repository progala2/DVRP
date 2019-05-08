﻿using System;
using System.Collections.Generic;
using System.Linq;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.Commons.Logging;
using _15pl04.Ucc.Commons.Utilities;
using _15pl04.Ucc.CommunicationServer.Components;
using _15pl04.Ucc.CommunicationServer.Components.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Models;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement
{
    /// <summary>
    /// Module responsible for storing information about problems/computations and assigning them to compatible cluster components.
    /// </summary>
    internal class WorkManager : IWorkManager
    {
        private static readonly ILogger Logger = new ConsoleLogger();
        private readonly IComponentOverseer _componentOverseer;
        private readonly Dictionary<Tuple<ulong, ulong>, PartialProblem> _partialProblems;
        private readonly Dictionary<Tuple<ulong, ulong>, PartialSolution> _partialSolutions;
        private readonly Dictionary<ulong, Problem> _problems;
        private readonly Random _random = new Random();
        private readonly Dictionary<ulong, Solution> _solutions;

        /// <summary>
        /// Creates WorkManager instance.
        /// </summary>
        /// <param name="componentOverseer">Component overseer managing components of whom work is scheduled.</param>
        public WorkManager(IComponentOverseer componentOverseer)
        {
            _componentOverseer = componentOverseer ?? throw new ArgumentNullException();
            _componentOverseer.Deregistration += OnComponentDeregistration;

            _problems = new Dictionary<ulong, Problem>();
            _solutions = new Dictionary<ulong, Solution>();
            _partialProblems = new Dictionary<Tuple<ulong, ulong>, PartialProblem>();
            _partialSolutions = new Dictionary<Tuple<ulong, ulong>, PartialSolution>();
        }

        /// <summary>
        /// Event that indicates assignment of computation/division/merge work to an appropriate cluster component.
        /// </summary>
        public event WorkAssignmentEventHandler WorkAssignment;

        /// <summary>
        /// Removes final solution from the system.
        /// </summary>
        /// <param name="problemId">Corresponding problem instance ID.</param>
        public void RemoveSolution(ulong problemId)
        {
            _solutions.Remove(problemId);
            Logger.Info("Removed solution from the server (id: " + problemId + ").");
        }

        /// <summary>
        /// Try get and assign work to the specified component.
        /// </summary>
        /// <param name="node">Node to assign work to.</param>
        /// <param name="work">Assigned work.</param>
        /// <returns>Whether there is any work compatible with the component.</returns>
        public bool TryAssignWork(SolverNodeInfo node, out Work work)
        {
            var type = node.ComponentType;
            var nodeId = node.ComponentId.Value;

            if (type == ComponentType.TaskManager)
            {
                var partialSolutionsToMerge = _partialSolutions.Values.Where(ps =>
                    ps.State == PartialSolution.PartialSolutionState.AwaitingMerge
                    && node.SolvableProblems.Contains(ps.PartialProblem.Problem.Type))
                    .ToList();

                if (partialSolutionsToMerge.Count != 0)
                {
                    var problemId = partialSolutionsToMerge[0].PartialProblem.Problem.Id;

                    var solutionsToAssign = partialSolutionsToMerge.Where(ps =>
                        ps.PartialProblem.Problem.Id == problemId)
                        .ToList();

                    foreach (var ps in solutionsToAssign)
                    {
                        ps.State = PartialSolution.PartialSolutionState.BeingMerged;
                        ps.MergingNodeId = nodeId;
                    }

                    work = new MergeWork(nodeId, solutionsToAssign);

                    var e = new WorkAssignmentEventArgs
                    {
                        AssigneeId = nodeId,
                        Work = work
                    };
                    WorkAssignment?.Invoke(this, e);

                    return true;
                }

                var problemToDivide = _problems.Values.FirstOrDefault(p =>
                    p.State == Problem.ProblemState.AwaitingDivision
                    && node.SolvableProblems.Contains(p.Type));

                if (problemToDivide != null)
                {
                    problemToDivide.State = Problem.ProblemState.BeingDivided;
                    problemToDivide.DividingNodeId = nodeId;

                    var availableThreads = CountAvailableSolvingThreads(problemToDivide.Type);

                    work = new DivisionWork(nodeId, problemToDivide, (ulong)availableThreads);

                    var e = new WorkAssignmentEventArgs
                    {
                        AssigneeId = nodeId,
                        Work = work
                    };
                    WorkAssignment?.Invoke(this, e);

                    return true;
                }

                work = null;
                return false;
            }
            if (type == ComponentType.ComputationalNode)
            {
                var availableThreads = node.ThreadInfo.Count(ts =>
                    ts.State == ThreadStatus.ThreadState.Idle);

                var partialProblemsToCompute = _partialProblems.Values.Where(pp =>
                    pp.State == PartialProblem.PartialProblemState.AwaitingComputation
                    && node.SolvableProblems.Contains(pp.Problem.Type)).ToList();

                if (availableThreads == 0 || !partialProblemsToCompute.Any())
                {
                    work = null;
                    return false;
                }

                var problemId = partialProblemsToCompute.First().Problem.Id;
                var problemsToAssign = new List<PartialProblem>(availableThreads);

                foreach (var pp in partialProblemsToCompute)
                {
                    if (availableThreads == 0)
                        break;

                    if (pp.Problem.Id == problemId)
                    {
                        pp.State = PartialProblem.PartialProblemState.BeingComputed;
                        pp.ComputingNodeId = nodeId;

                        problemsToAssign.Add(pp);
                        availableThreads--;
                    }
                }

                work = new ComputationWork(nodeId, problemsToAssign);

                var e = new WorkAssignmentEventArgs
                {
                    AssigneeId = nodeId,
                    Work = work
                };
                WorkAssignment?.Invoke(this, e);

                return true;
            }
            throw new InvalidOperationException();
        }
        /// <summary>
        /// Adds new problem instance to the system.
        /// </summary>
        /// <param name="type">Type name of the problem.</param>
        /// <param name="data">Problem data.</param>
        /// <param name="solvingTimeout">Timeout </param>
        /// <returns>ID assigned to the problem instance.</returns>
        public ulong AddProblem(string type, byte[] data, ulong solvingTimeout)
        {
            // Generate problem id.
            ulong id;
            do
            {
                id = _random.NextUInt64() % 100; //TODO this is debug solution
                //id = _random.NextUInt64(); 
            } while (_problems.ContainsKey(id));

            // Create problem instance.
            var problem = new Problem(id, type, data, solvingTimeout)
            {
                State = Problem.ProblemState.AwaitingDivision,
                DividingNodeId = null
            };

            // Add problem to the set.
            _problems.Add(id, problem);

            Logger.Info("Added new problem (id: " + id + ", type: " + type + ").");
            return id;
        }
        /// <summary>
        /// Adds new partial problem to the system.
        /// </summary>
        /// <param name="problemId">ID of the problem instance this partial problem belongs to.</param>
        /// <param name="partialProblemId">ID of the partial problem withing the problem instance.</param>
        /// <param name="privateData">Partial problem private data.</param>
        public void AddPartialProblem(ulong problemId, ulong partialProblemId, byte[] privateData)
        {
            // Make sure the corresponding problem instance exists.
            if (!_problems.TryGetValue(problemId, out var problem))
            {
                Logger.Error("Corresponding problem instance doesn't exist.");
                return;
            }

            // Make sure that state of the corresponding problem instance is set to "being divided".
            if (problem.State != Problem.ProblemState.BeingDivided)
            {
                Logger.Error("Corresponding problem's state is invalid.");
                return;
            }

            // Make sure the corresponding problem's property 'NumberOfParts' is set to a positive integer.
            if (!problem.NumberOfParts.HasValue || problem.NumberOfParts < 1)
            {
                Logger.Error("Corresponding problem's 'NumberOfParts' property is not a positive integer.");
                return;
            }

            // Make sure the problem instance's number of partial problems & partial solutions is not greater than 'NumberOfParts' (excluding the partial problem being added).
            var ppNum = _partialProblems.Count(pair => pair.Key.Item1 == problemId);
            var psNum = _partialSolutions.Count(pair => pair.Key.Item1 == problemId);
            if (ppNum + psNum == (int)problem.NumberOfParts - 1)
            {
                Logger.Info("Got all partial problems (id: " + problem.Id + ").");
                problem.State = Problem.ProblemState.AwaitingSolution;
            }
            else if (ppNum + psNum > (int)problem.NumberOfParts - 1)
            {
                Logger.Warn("Received more partial problems than expected.");
                return;
            }

            // Create partial problem instance.
            var partialProblem = new PartialProblem(partialProblemId, problem, privateData)
            {
                State = PartialProblem.PartialProblemState.AwaitingComputation,
                ComputingNodeId = null
            };

            // Add partial problem to the set.
            var pairId = new Tuple<ulong, ulong>(problemId, partialProblemId);
            _partialProblems.Add(pairId, partialProblem);

            Logger.Info("Added new partial problem (id: " + problemId + "/" + partialProblemId + ", type: " +
                        problem.Type + ").");
        }
        /// <summary>
        /// Adds final solution to the system.
        /// </summary>
        /// <param name="problemId">ID of the solved problem instance.</param>
        /// <param name="data">Solution data.</param>
        /// <param name="computationsTime">Total time of problem computations.</param>
        /// <param name="timeoutOccured">Whether timeout stopped the computations.</param>
        public void AddSolution(ulong problemId, byte[] data, ulong computationsTime, bool timeoutOccured)
        {
            // Make sure the corresponding problem instance exists.
            if (!_problems.ContainsKey(problemId))
            {
                Logger.Error("Corresponding problem instance doesn't exist.");
                return;
            }

            var problem = _problems[problemId];

            // Make sure that state of the corresponding problem instance is set to "awaiting solution".
            if (problem.State != Problem.ProblemState.AwaitingSolution)
            {
                Logger.Error("Corresponding problem's state is invalid.");
                return;
            }

            // Make sure there are partial solutions in "being merged" state. Delete them.
            var mergedPartialSolutions = GetPartialSolutions(problemId, PartialSolution.PartialSolutionState.BeingMerged);
            foreach (var ps in mergedPartialSolutions)
            {
                var pairId = new Tuple<ulong, ulong>(problemId, ps.PartialProblem.Id);
                _partialSolutions.Remove(pairId);
            }
            if (mergedPartialSolutions.Count == 0)
                Logger.Warn("There are no corresponding partial solutions in 'being merged' state.");

            // Create solution instance.
            var solution = new Solution(problem, data, computationsTime, timeoutOccured);

            // Add solution to the set.
            _solutions.Add(problemId, solution);

            // Delete corresponding problem.
            _problems.Remove(problemId);

            // Perform cleanup if any partial problem/solution related to the problem, is left.
            _partialProblems.RemoveAll((tuple, pp) =>
            {
                if (tuple.Item1 != problemId)
                    return false;

                Logger.Warn("Found a partial problem left out during the clean-up.");
                return true;
            });
            _partialSolutions.RemoveAll((tuple, ps) =>
            {
                if (tuple.Item1 != problemId)
                    return false;

                Logger.Warn("Found a partial solution left out during the clean-up.");
                return true;
            });

            Logger.Info("Added new solution (id: " + problemId + ", type: " + problem.Type + ").");
        }
        /// <summary>
        /// Adds new partial solution to the system.
        /// </summary>
        /// <param name="problemId">ID of the corresponding problem instance.</param>
        /// <param name="partialProblemId">ID of the corresponding partial problem.</param>
        /// <param name="data">Partial solution data.</param>
        /// <param name="computationsTime">Time of the foregoing computations.</param>
        /// <param name="timeoutOccured">Whether timeout stopped the computations.</param>
        public void AddPartialSolution(ulong problemId, ulong partialProblemId, byte[] data, ulong computationsTime,
            bool timeoutOccured)
        {
            var pairId = new Tuple<ulong, ulong>(problemId, partialProblemId);

            // Make sure the corresponding problem instance exists.
            if (!_problems.ContainsKey(problemId))
            {
                Logger.Error("Corresponding problem instance doesn't exist.");
                return;
            }

            var problem = _problems[problemId];

            // Make sure the corresponding partial problem instance exists.
            if (!_partialProblems.ContainsKey(pairId))
            {
                Logger.Error("Corresponding partial problem doesn't exist.");
                return;
            }

            var partialProblem = _partialProblems[pairId];

            // Make sure that state of the corresponding partial problem is set to "being computed".
            if (partialProblem.State != PartialProblem.PartialProblemState.BeingComputed)
            {
                Logger.Error("Corresponding partial problem's state is invalid.");
                return;
            }

            // Create partial solution instance.
            var partialSolution = new PartialSolution(partialProblem, data, computationsTime, timeoutOccured)
            {
                State = PartialSolution.PartialSolutionState.BeingGathered,
                MergingNodeId = null
            };

            // Add partial solution to the set.
            _partialSolutions.Add(pairId, partialSolution);

            // Delete corresponding partial problem.
            _partialProblems.Remove(pairId);

            Logger.Info("Added new partial solution (id: " + problemId + "/" + partialProblemId + ", type: " +
                        problem.Type + ").");

            // Check if all partial solutions are in and set appropriate state.
            var gatheredPartialSolutions = GetPartialSolutions(problemId,
                PartialSolution.PartialSolutionState.BeingGathered);
            if (problem.NumberOfParts != null && (int)problem.NumberOfParts == gatheredPartialSolutions.Count)
            {
                foreach (var ps in gatheredPartialSolutions)
                    ps.State = PartialSolution.PartialSolutionState.AwaitingMerge;

                Logger.Info("Partial solutions are ready to merge (id: " + problemId + ").");
            }
        }
        /// <summary>
        /// Gets final solution data by problem instance ID.
        /// </summary>
        /// <param name="problemId">Problem instance (solution) ID.</param>
        /// <returns>Final solution.</returns>
        public Solution GetSolution(ulong problemId)
        {
            return _solutions.ContainsKey(problemId) ? _solutions[problemId] : null;
        }
        /// <summary>
        /// Gets problem instance information by ID.
        /// </summary>
        /// <param name="problemId">Problem instance ID.</param>
        /// <returns>Problem instance.</returns>
        public Problem GetProblem(ulong problemId)
        {
            if (_problems.ContainsKey(problemId))
                return _problems[problemId];
            return null;
        }
        /// <summary>
        /// Gets foregoing computations time for the problem instance specified by ID.
        /// </summary>
        /// <param name="problemId">ID of the problem instance.</param>
        /// <returns>Computations time in milliseconds.</returns>
        public ulong GetComputationsTime(ulong problemId)
        {
            if (_solutions.ContainsKey(problemId))
                return _solutions[problemId].ComputationsTime;
            return 0;
        }
        /// <summary>
        /// Gets ID of the node that is currently processing (partial) problem specified by its ID.
        /// </summary>
        /// <param name="problemId">ID of the problem instance.</param>
        /// <param name="partialProblemId">ID of the partial problem within the problem instance. Can be null.</param>
        /// <returns>Problem or partial problem ID depending on partialProblemId value. Null if (partial)problem not found or isn't processed by any component.</returns>
        public ulong? GetProcessingNodeId(ulong problemId, ulong? partialProblemId = null)
        {
            ulong? nodeId = null;

            if (partialProblemId == null)
            {
                if (_problems.ContainsKey(problemId))
                    nodeId = _problems[problemId].DividingNodeId;
            }
            else
            {
                var pairId = new Tuple<ulong, ulong>(problemId, partialProblemId.Value);

                if (_partialProblems.ContainsKey(pairId))
                    nodeId = _partialProblems[pairId].ComputingNodeId;

                if (!nodeId.HasValue && _partialSolutions.ContainsKey(pairId))
                    nodeId = _partialSolutions[pairId].MergingNodeId;
            }

            return nodeId;
        }
        /// <summary>
        /// Handles component's de-registration by reclaiming all the work it's currently been doing.
        /// </summary>
        /// <param name="sender">Component overseer that invoked the event.</param>
        /// <param name="e">Information about the deregistration.</param>
        private void OnComponentDeregistration(object sender, DeregisterationEventArgs e)
        {
            var componentType = e.Component.ComponentType;

            if (componentType == ComponentType.CommunicationServer)
            {
                var backup = e.Component as BackupServerInfo;

                throw new NotImplementedException();
                // TODO implement
            }
            if (componentType == ComponentType.TaskManager)
            {
                var component = e.Component as SolverNodeInfo;

                var problemsBeingDivided = _problems.Values.Where(p =>
                    p.DividingNodeId == component.ComponentId
                    && p.State == Problem.ProblemState.BeingDivided);

                foreach (var p in problemsBeingDivided)
                {
                    p.DividingNodeId = null;
                    p.State = Problem.ProblemState.AwaitingDivision;
                }

                var partialSolutionsBeingMerged = _partialSolutions.Values.Where(ps =>
                    ps.MergingNodeId == component.ComponentId
                    && ps.State == PartialSolution.PartialSolutionState.BeingMerged);

                foreach (var ps in partialSolutionsBeingMerged)
                {
                    ps.MergingNodeId = null;
                    ps.State = PartialSolution.PartialSolutionState.AwaitingMerge;
                }
            }
            else if (componentType == ComponentType.ComputationalNode)
            {
                var component = e.Component as SolverNodeInfo;

                var partialProblemsBeingComputed = _partialProblems.Values.Where(pp =>
                    pp.ComputingNodeId == component.ComponentId
                    && pp.State == PartialProblem.PartialProblemState.BeingComputed);

                foreach (var pp in partialProblemsBeingComputed)
                {
                    pp.ComputingNodeId = null;
                    pp.State = PartialProblem.PartialProblemState.AwaitingComputation;
                }
            }
        }
        /// <summary>
        /// Gets the total number of threads within the system that can compute the specified problem type.
        /// </summary>
        /// <param name="problemType">Type name of the problem class.</param>
        /// <returns>Number of threads.</returns>
        private int CountAvailableSolvingThreads(string problemType)
        {
            var availableThreads = _componentOverseer.GetComponents(ComponentType.ComputationalNode)
                .Cast<SolverNodeInfo>()
                .Where(sn => sn.SolvableProblems.Contains(problemType))
                .Select(sn => sn.ThreadInfo.Count(ts => ts.State == ThreadStatus.ThreadState.Idle)).Sum();

            // No available threads found be we still need to divide the problem into some parts.
            if (availableThreads == 0)
                availableThreads = 3;

            return availableThreads;
        }
        /// <summary>
        /// Gets partial problems belonging to the problem instance specified by ID and in (optionally) specified state.
        /// </summary>
        /// <param name="problemId">ID of the corresponding problem instance.</param>
        /// <param name="state">Partial problem state filter. Can be null.</param>
        /// <returns>Collection of matching partial problems.</returns>
        private ICollection<PartialProblem> GetPartialProblems(ulong problemId,
            PartialProblem.PartialProblemState? state = null)
        {
            return _partialProblems.Values.Where(pp =>
            {
                var pId = pp.Problem.Id;

                if (state.HasValue)
                    return problemId == pId && state == pp.State;
                return problemId == pId;
            }).ToList();
        }
        /// <summary>
        /// Gets partial solutions by the corresponding problem instance ID and in (optionally) specified state.
        /// </summary>
        /// <param name="problemId">ID of the corresponding problem instance.</param>
        /// <param name="state">Partial solution state filter. Can be null.</param>
        /// <returns>Collection of matching partial solutions.</returns>
        private ICollection<PartialSolution> GetPartialSolutions(ulong problemId,
            PartialSolution.PartialSolutionState? state = null)
        {
            return _partialSolutions.Values.Where(ps =>
            {
                var pId = ps.PartialProblem.Problem.Id;

                if (state.HasValue)
                    return problemId == pId && state == ps.State;
                return problemId == pId;
            }).ToList();
        }
    }
}