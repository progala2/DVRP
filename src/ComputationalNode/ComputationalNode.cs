using System;
using System.Net;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.ComputationalNode
{
    public class ComputationalNode : ComputationalComponent
    {
        public ComputationalNode(IPEndPoint serverAddress)
            : base(serverAddress)
        { }


        protected override RegisterMessage GetRegisterMessage()
        {
            // create RegisterMessage proper for ComputationalNode
            throw new NotImplementedException();
        }

        protected override void HandleResponseMessage(Message message)
        {
            throw new NotImplementedException();
            // switch over possible messages

            // run computations with:
            // this.ComputationalTaskPool.StartComputationalTask(...);

            // like:
            //PartialProblemsMessage ppmsg = (PartialProblemsMessage)message;
            //this.ComputationalTaskPool.StartComputationalTask(()=>PartialProblemsMessage(ppmsg),ppmsg.ProblemType,...)
        }

        private void PartialProblemsMessageHandler(PartialProblemsMessage message)
        {
            throw new NotImplementedException();
            // get proper TaskSolver by
            var taskSolver = this.TaskSolvers[message.ProblemType];

            // solve problem

            // add proper message to send
            //this.EnqueueMessageToSend(...);
        }
    }
}
