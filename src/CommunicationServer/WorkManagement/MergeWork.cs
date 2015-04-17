using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Models;
using System;
using System.Collections.Generic;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement
{
    public class MergeWork : Work
    {
        public List<PartialSolution> PartialSolutions 
        { 
            get; 
            private set; 
        }
        public override ulong AssigneeId
        {
            get;
            protected set;
        }
        public override WorkType Type
        {
            get { return WorkType.Merge; }
        }


        public MergeWork(ulong assigneeId, IList<PartialSolution> partialSolutions)
        {
            if (partialSolutions == null || partialSolutions.Count == 0)
                throw new ArgumentException();

            ulong problemId = partialSolutions[0].PartialProblem.Problem.Id.Value;
            foreach (PartialSolution ps in partialSolutions)
            {
                if (problemId != ps.PartialProblem.Problem.Id)
                    throw new ArgumentException("All partial solutions must belong to the same problem instance.");
            }

            AssigneeId = assigneeId; 
            PartialSolutions = new List<PartialSolution>(partialSolutions);
        }

        public override Message CreateMessage()
        {
            var msgPartialSolutions = new List<SolutionsMessage.Solution>(PartialSolutions.Count);

            foreach (PartialSolution ps in PartialSolutions)
                msgPartialSolutions.Add((SolutionsMessage.Solution)ps);


            Problem problemInstance = PartialSolutions[0].PartialProblem.Problem;

            var message = new SolutionsMessage()
            {
                CommonData = PartialSolutions[0].PartialProblem.CommonData,
                ProblemInstanceId = problemInstance.Id.Value,
                ProblemType = problemInstance.ProblemType,
                Solutions = msgPartialSolutions,
            };

            return message;
        }

    }
}
