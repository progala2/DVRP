using _15pl04.Ucc.CommunicationServer.WorkManagement.Models;
using System;
using System.Collections.Generic;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement.Base
{
    public delegate void WorkAssignmentEventHandler(object sender, WorkAssignmentEventArgs e);

    public interface IWorkManager
    {
        event WorkAssignmentEventHandler WorkAssignment;

        bool TryGetWork(ulong nodeId, out Work work);
        bool TryGetSolution(ulong problemId, out Solution solution);

        ulong AddProblem(Problem problem);
        void AddPartialProblems(IList<PartialProblem> partialProblems);
        void AddSolution(Solution solution);
        void AddPartialSolutions(IList<PartialSolution> partialSolutions);
    }

    public class WorkAssignmentEventArgs : EventArgs
    {
        public Work Work { get; set; }
        public ulong AssigneeId { get; set; }
    }
}
