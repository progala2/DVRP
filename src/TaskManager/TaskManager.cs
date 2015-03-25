using System;
using System.Net;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.TaskManager
{
    public sealed class TaskManager : ComputationalComponent
    {
        public TaskManager(IPEndPoint serverAddress)
            : base(serverAddress)
        { }

        protected override RegisterMessage GetRegisterMessage()
        {
            // create RegisterMessage proper for TaskManager
            throw new NotImplementedException();
        }

        protected override void HandleResponseMessage(Message message)
        {
            throw new NotImplementedException();

            // switch over possible message types

            /*                          
            //possible adding new task to compute like:
            this.StartComputationalTask(() =>
            {
                this._taskSolvers[problemName].MergeSolution(someDataFromMessage);
              this.EnqueueMessageToSend(messageWithFinalSolution);
            }, problemName, problemInstanceId, null);
             */
        }
    }
}
