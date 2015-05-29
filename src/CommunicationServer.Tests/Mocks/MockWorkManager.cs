using System;
using _15pl04.Ucc.CommunicationServer.Components;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Models;

namespace _15pl04.Ucc.CommunicationServer.Tests.Mocks
{
    internal class MockWorkManager : IWorkManager
    {
        public event WorkAssignmentEventHandler WorkAssignment;

        public bool TryAssignWork(SolverNodeInfo node, out Work work)
        {
            throw new NotImplementedException();
        }

        public ulong AddProblem(string type, byte[] data, ulong solvingTimeout)
        {
            throw new NotImplementedException();
        }

        public void AddPartialProblem(ulong problemId, ulong partialProblemId, byte[] privateData)
        {
            throw new NotImplementedException();
        }

        public void AddSolution(ulong problemId, byte[] data, ulong computationsTime, bool timeoutOccured)
        {
            throw new NotImplementedException();
        }

        public void AddPartialSolution(ulong problemId, ulong partialProblemId, byte[] data, ulong computationsTime,
            bool timeoutOccured)
        {
            throw new NotImplementedException();
        }

        public Problem GetProblem(ulong problemId)
        {
            throw new NotImplementedException();
        }

        public Solution GetSolution(ulong problemId)
        {
            throw new NotImplementedException();
        }

        public ulong GetComputationsTime(ulong problemId)
        {
            throw new NotImplementedException();
        }

        public ulong? GetProcessingNodeId(ulong problemId, ulong? partialProblemId = null)
        {
            throw new NotImplementedException();
        }

        public void RemoveSolution(ulong problemId)
        {
            throw new NotImplementedException();
        }
    }
}