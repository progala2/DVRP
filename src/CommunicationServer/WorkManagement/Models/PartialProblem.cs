using _15pl04.Ucc.Commons.Messaging.Models;
using System;

namespace _15pl04.Ucc.CommunicationServer.WorkManagement.Models
{
    public class PartialProblem
    {
        public Problem Problem
        {
            get;
            private set;
        }
        public ulong Id
        {
            get;
            private set;
        }
        public byte[] PrivateData
        {
            get;
            private set;
        }
        public byte[] CommonData
        {
            get;
            private set;
        }
        public ulong DividingTaskManagerId
        {
            get;
            private set;
        }


        public PartialProblem(Problem problem, ulong id, 
            byte[] privateData, byte[] commonData, ulong dividingTaskManagerId)
        {
            if (!problem.Id.HasValue)
                throw new Exception("Problem has no id set.");

            Problem = problem;
            Id = id;
            PrivateData = privateData;
            CommonData = commonData;
            DividingTaskManagerId = dividingTaskManagerId;
        }

        public static explicit operator PartialProblemsMessage.PartialProblem(PartialProblem pp)
        {
            var output = new PartialProblemsMessage.PartialProblem()
            {
                Data = pp.PrivateData,
                PartialProblemId = pp.Id,
                TaskManagerId = pp.DividingTaskManagerId,
            };

            return output;
        }
    }
}
