using _15pl04.Ucc.CommunicationServer.Components;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Models;
using System.Collections.Generic;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement.Base
{
    public interface IWorkManager
    {
        bool TryGetWork(SolverNodeInfo node, out Work work);
        bool TryGetSolution(ulong problemId, out Solution solution);


        ulong AddProblem(Problem problem);
        void AddPartialProblems(ICollection<PartialProblem> partialProblems);
        void AddSolution(Solution solution);
        void AddPartialSolutions(ICollection<PartialSolution> partialSolutions);
    }
}
