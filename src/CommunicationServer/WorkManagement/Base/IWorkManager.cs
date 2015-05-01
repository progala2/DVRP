using _15pl04.Ucc.CommunicationServer.Components;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Models;
using System;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement.Base
{
    public delegate void WorkAssignmentEventHandler(object sender, WorkAssignmentEventArgs e);

    internal interface IWorkManager
    {
        event WorkAssignmentEventHandler WorkAssignment;

        bool TryAssignWork(SolverNodeInfo node, out Work work);

        ulong AddProblem(string type, byte[] data, ulong solvingTimeout);
        void AddPartialProblem(ulong problemId, ulong partialProblemId, byte[] privateData);
        void AddSolution(ulong problemId, byte[] data, ulong computationsTime, bool timeoutOccured);
        void AddPartialSolution(ulong problemId, ulong partialProblemId, byte[] data, ulong computationsTime, bool timeoutOccured);

        Problem GetProblem(ulong problemId);
        Solution GetSolution(ulong problemId);
        ulong GetComputationsTime(ulong problemId);
        ulong? GetProcessingNodeId(ulong problemId, ulong? partialProblemId = null);

        void RemoveSolution(ulong problemId);
    }

    public class WorkAssignmentEventArgs : EventArgs
    {
        public Work Work { get; set; }
        public ulong AssigneeId { get; set; }
    }
}
