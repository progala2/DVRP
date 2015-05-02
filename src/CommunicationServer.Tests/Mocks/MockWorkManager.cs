using _15pl04.Ucc.CommunicationServer.WorkManagement.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _15pl04.Ucc.CommunicationServer.Components;

namespace _15pl04.Ucc.CommunicationServer.Tests.Mocks
{
    class MockWorkManager : IWorkManager
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

        public void AddPartialSolution(ulong problemId, ulong partialProblemId, byte[] data, ulong computationsTime, bool timeoutOccured)
        {
            throw new NotImplementedException();
        }

        public WorkManagement.Models.Problem GetProblem(ulong problemId)
        {
            throw new NotImplementedException();
        }

        public WorkManagement.Models.Solution GetSolution(ulong problemId)
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
