using _15pl04.Ucc.CommunicationServer.Collections;
using _15pl04.Ucc.CommunicationServer.Components.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Models;
using System;
using System.Collections.Generic;
using _15pl04.Ucc.Commons.Utilities;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.CommunicationServer.Components;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement
{
    public class WorkManager : IWorkManager
    {
        public event WorkAssignmentEventHandler WorkAssignment;

        private IComponentOverseer _componentOverseer;

        private Random _random = new Random();
        private HashSet<ulong> _problemIdsInUse = new HashSet<ulong>();

        #region Problem & solution instance containers

        private LexicographicQueue<string, Problem> _problemsAwaitingDivision;                  // <ProblemType, P>
        private LexicographicQueue<ulong, Problem> _problemsBeingDivided;                       // <TaskManagerId, P>
        private Dictionary<ulong, Problem> _problemsAwaitingSolution;                           // <ProblemId, P>

        private LexicographicQueue<string, PartialProblem> _partialProblemsAwaitingComputation; // <ProblemType, PP>
        private LexicographicQueue<ulong, PartialProblem> _partialProblemsBeingComputed;        // <ComputationalNodeId, PP>

        private LexicographicQueue<ulong, PartialSolution> _partialSolutionsBeingGathered;      // <ProblemId, PS>
        private LexicographicQueue<string, PartialSolution[]> _partialSolutionsAwaitingMerge;   // <ProblemType, PS>
        private LexicographicQueue<ulong, PartialSolution[]> _partialSolutionsBeingMerged;      // <TaskManagerId, PS>

        private Dictionary<ulong, Solution> _finalSolutions;                                    // <ProblemId, S>

        #endregion

        public WorkManager(IComponentOverseer co)
        {
            _componentOverseer = co;
            _componentOverseer.Deregistration += OnComponentDeregistration;

            #region Containers' initialization

            _problemsAwaitingDivision = new LexicographicQueue<string, Problem>();
            _problemsBeingDivided = new LexicographicQueue<ulong, Problem>();
            _problemsAwaitingSolution = new Dictionary<ulong, Problem>();

            _partialProblemsAwaitingComputation = new LexicographicQueue<string, PartialProblem>();
            _partialProblemsBeingComputed = new LexicographicQueue<ulong, PartialProblem>();

            _partialSolutionsBeingGathered = new LexicographicQueue<ulong, PartialSolution>();
            _partialSolutionsAwaitingMerge = new LexicographicQueue<string, PartialSolution[]>();
            _partialSolutionsBeingMerged = new LexicographicQueue<ulong, PartialSolution[]>();

            _finalSolutions = new Dictionary<ulong, Solution>();

            #endregion


        }

        #region Getting work


        public bool TryGetWork(ulong nodeId, out Work work)
        {
            if (!_componentOverseer.IsRegistered(nodeId))
                throw new ArgumentException("No registered component with the specified id is found.");



            throw new System.NotImplementedException();
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
            do{
                id = _random.NextUInt64();
            }while (_problemIdsInUse.Contains(id));

            problem.Id = id; // Will throw an exception if Id is already set.
            _problemIdsInUse.Add(id);

            // Mark as "awaiting division".
            _problemsAwaitingDivision.Enqueue(problem.ProblemType, problem);

            return id;
        }

        public void AddPartialProblems(IList<PartialProblem> partialProblems)
        {
            if (partialProblems == null)
                throw new ArgumentNullException();

            if (partialProblems.Count == 0)
                throw new ArgumentException();

            Problem problem = partialProblems[0].Problem;

            //TODO check if there actually is a problem being divided
            //TODO remove it

            foreach (PartialProblem pp in partialProblems)
                if (pp.Problem.Id != problem.Id)
                    throw new Exception("All partial problems must belong to the same problem instance.");

            _partialProblemsAwaitingComputation.Enqueue(problem.ProblemType, (Queue<PartialProblem>)partialProblems);
        }

        public void AddSolution(Solution solution)
        {
            if (solution == null)
                throw new ArgumentNullException();

            ulong problemId = solution.Problem.Id.Value;

            if (_problemsAwaitingSolution.Remove(problemId))
            {
                _finalSolutions.Add(problemId, solution);
            }
            else
            {
                throw new Exception("There is no corresponding problem instance awaiting for a solution.");
            }
        }

        public void AddPartialSolutions(IList<PartialSolution> partialSolutions)
        {
            throw new System.NotImplementedException();
        }


        #endregion

        private void OnComponentDeregistration(object sender, DeregisterationEventArgs e)
        {
            ulong id = e.Component.ComponentId.Value;

            switch (e.Component.ComponentType)
            {
                case ComponentType.TaskManager:
                    Queue<Problem> problemsBeingDivided = _problemsBeingDivided.RemoveAllByKey(id);
                    if (problemsBeingDivided != null)
                    {
                        foreach (Problem p in problemsBeingDivided)
                        {
                            p.NumberOfParts = null;
                            _problemsAwaitingDivision.Enqueue(p.ProblemType, p);
                        }
                    }

                    Queue<PartialSolution[]> solutionsBeingMerged = _partialSolutionsBeingMerged.RemoveAllByKey(id);
                    if (solutionsBeingMerged != null)
                    {
                        foreach (PartialSolution[] psArray in solutionsBeingMerged)
                        {
                            string problemType = psArray[0].PartialProblem.Problem.ProblemType;
                            _partialSolutionsAwaitingMerge.Enqueue(problemType, psArray);
                        }
                    }
                    break;

                case ComponentType.ComputationalNode:
                    Queue<PartialProblem> problemsBeingComputed = _partialProblemsBeingComputed.RemoveAllByKey(id);
                    if (problemsBeingComputed != null)
                    {
                        foreach (PartialProblem pp in problemsBeingComputed)
                        {
                            string problemType = pp.Problem.ProblemType;
                            _partialProblemsAwaitingComputation.Enqueue(problemType, pp);
                        }
                    }
                    break;

                case ComponentType.CommunicationServer:
                    BackupServerInfo backupInfo = e.Component as BackupServerInfo;

                    // TODO
                    break;
            }
        }
    }
}
