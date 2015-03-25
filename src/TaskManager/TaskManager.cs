using System;
using System.Net;
using System.Threading.Tasks;
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
             
            // possible adding new task to compute like:                        
            var task = new Task(() =>
            {
                this._taskSolvers[problemName].MergeSolution(someDataFromMessage);
                
                // IT IS IMPORTANT to release Task in array at the end of task,
                // this will change State to Idle and update LastStateChange time
                // probably it will be refactored
                this._computationalTasks[availableIndex].Task = null;
            });
            var computationalTask = new ComputationalTask(task)
            {
                // adding data needed for getting StatusMessage:
                ProblemInstanceId=,
                PartialProblemId=,
                ProblemType=,
            };
             
             // get availableIndex - search _computationalTasks for ComputationalTask with Idle state
             // (if we received message with data to compute then there should be at least one Idle ComputationalTask)
            this._computationalTasks[availableIndex] = computationalTask;
            // and start it
            this._computationalTasks[availableIndex].Task.Start();
             
            */
        }
    }
}
