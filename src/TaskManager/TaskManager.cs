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
             
             // get availableIndex - search _computationalTasks for ComputationalTask with Idle state
             // (if we received message with data to compute then there should be at least one Idle ComputationalTask)
            int availableIndex = ...;
             
            // possible adding new task to compute like:                        
            var task = new Task(() =>
            {
                this._taskSolvers[problemName].MergeSolution(someDataFromMessage);
                
                // IT IS IMPORTANT to release Task in array at the end of task,
                // creating new ComputationalTask will set default values like: change State to Idle etc.
                // probably it will be refactored
                this._computationalTasks[availableIndex] = new ComputationalTask();
            });
            var computationalTask = new ComputationalTask(task)
            {
                // adding data needed for getting StatusMessage:
                ProblemInstanceId=,
                PartialProblemId=,
                ProblemType=,
            };       
             
             this._computationalTasks[availableIndex] = computationalTask;
            // and start it
            this._computationalTasks[availableIndex].Task.Start();
             
            */
        }
    }
}
