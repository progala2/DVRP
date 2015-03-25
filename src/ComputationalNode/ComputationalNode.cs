using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using UCCTaskSolver;

namespace _15pl04.Ucc.ComputationalNode
{
    public class ComputationalNode : ComputationalComponent
    {
        public ComputationalNode(IPEndPoint serverAddress)
            : base(serverAddress)
        { }


        protected override RegisterMessage GetRegisterMessage()
        {
            throw new NotImplementedException();
        }

        protected override void HandleResponseMessage(Message message)
        {
            throw new NotImplementedException();
            /* if it is a PartialProblemsMessage
             * start new task running SolvePartialProblem method with available taskIndex
             * (threadIndex is available if _computationalTasks[taskIndex].State == Idle)
             */
            // update information in _computationalTasks[availableTaskIndex] needed for getting Status message (ProblemInstanceId,PartialProblemId,ProblemType)
            //_computationalTasks[availableTaskIndex].Task = new Task(()=>SolvePartialProblem(availableTaskIndex,ppmsg),_cancellationTokenSource.Token);
        
        }

        private void SolvePartialProblem(int taskIndex, PartialProblemsMessage message)
        {
            throw new NotImplementedException();
            // get proper TaskSolver by
            // _taskSolvers["nameOfProblem"]


            // solve problem


            // add proper message to send
            // _messagesToSend.Enqueue(...);

            // release computation task (create new in idle state)
            _computationalTasks[taskIndex] = new ComputationalTask();
        }
    }
}
