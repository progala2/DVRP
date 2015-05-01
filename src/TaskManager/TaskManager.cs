﻿using System;
using System.Collections.Generic;
using System.Net;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Computations;
using _15pl04.Ucc.Commons.Messaging.Models;
using UCCTaskSolver;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.Commons.Computations.Base;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.TaskManager
{
    public sealed class TaskManager : ComputationalComponent
    {
        public override ComponentType ComponentType
        {
            get { return ComponentType.TaskManager; }
        }


        /// <summary>
        /// Creates TaskManager which looks for task solvers in current directory.
        /// </summary>
        /// <param name="threadManager">The thread manager. Cannot be null.</param>
        /// <param name="serverAddress">The primary server address. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public TaskManager(ThreadManager threadManager, IPEndPoint serverAddress)
            : base(threadManager, serverAddress)
        {
        }

        /// <summary>
        /// Creates TaskManager.
        /// </summary>
        /// <param name="threadManager">The thread manager. Cannot be null.</param>
        /// <param name="serverAddress">The primary server address. Cannot be null.</param>
        /// <param name="taskSolversDirectoryRelativePath">The relative path to directory with task solvers.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        public TaskManager(ThreadManager threadManager, IPEndPoint serverAddress, string taskSolversDirectoryRelativePath)
            : base(threadManager, serverAddress, taskSolversDirectoryRelativePath)
        {
        }


        /// <summary>
        /// Handles any message received from server after registration process completes successfully.
        /// </summary>
        /// <param name="message">Message to handle.</param>
        /// <remarks>
        /// RegisterResponse is handled in base class.
        /// </remarks>
        protected override void HandleReceivedMessage(Message message)
        {
            switch (message.MessageType)
            {
                case MessageClass.NoOperation:
                    NoOperationMessageHandler((NoOperationMessage)message);
                    break;
                case MessageClass.DivideProblem:
                    DivideProblemMessageHandler((DivideProblemMessage)message);
                    break;
                case MessageClass.Solutions:
                    SolutionsMessageHandler((SolutionsMessage)message);
                    break;
                case MessageClass.Error:
                    ErrorMessageHandler((ErrorMessage)message);
                    break;
                default:
                    throw new InvalidOperationException("Received not supported message type.");
            }
        }

        private void NoOperationMessageHandler(NoOperationMessage message)
        {
            // nothing to do, backuping is handled by MessageSender
        }

        /// <exception cref="System.InvalidOperationException">Thrown when:
        /// - message is designated for TaskManger with different ID,
        /// - problem type can't be solved with this TaskManger,
        /// - dividing problem cannot be started bacause no tasks are available in task pool.</exception>
        private void DivideProblemMessageHandler(DivideProblemMessage message)
        {
            if (ID != message.TaskManagerId)
            {
                // shouldn't ever get here - received message for other TaskManager
                throw new InvalidOperationException(string.Format("TaskManager manager with ID={0} received message for TaskManager with ID={1}.", ID, message.TaskManagerId));
            }
            if (!TaskSolvers.ContainsKey(message.ProblemType))
            {
                // shouldn't ever get here - received unsolvable problem
                throw new InvalidOperationException(string.Format("\"{0}\" problem type can't be divided with this TaskManager.", message.ProblemType));
            }
            var taskSolverType = TaskSolvers[message.ProblemType];

            // should be started properly cause server sends at most as many tasks to do as count of component's tasks in idle state
            bool started = ThreadManager.StartInNewThread(() =>
            {
                var taskSolver = (TaskSolver)Activator.CreateInstance(taskSolverType, message.ProblemData);

                // measure time using DateTime cause StopWatch is not guaranteed to be thread safe
                var start = DateTime.UtcNow;
                var partialProblemsData = taskSolver.DivideProblem((int)message.ComputationalNodes);
                var stop = DateTime.UtcNow;

                var partialProblems = new List<PartialProblemsMessage.PartialProblem>(partialProblemsData.GetLength(0));
                for (int i = 0; i < partialProblemsData.GetLength(0); i++)
                {
                    partialProblems.Add(new PartialProblemsMessage.PartialProblem()
                    {
                        PartialProblemId = (ulong)i,
                        Data = partialProblemsData[i],
                        TaskManagerId = ID,
                    });
                }

                var partialProblemsMessage = new PartialProblemsMessage()
                {
                    ProblemType = message.ProblemType,
                    ProblemInstanceId = message.ProblemInstanceId,
                    CommonData = message.ProblemData,
                    PartialProblems = partialProblems,
                };

                EnqueueMessageToSend(partialProblemsMessage);
            }, message.ProblemType, message.ProblemInstanceId, null);
            if (!started)
            {
                // tragedy, CommunicationServer surprised us like the Spanish Inquisition
                throw new InvalidOperationException("Couldn't divide problem bacause no tasks are available in task pool.");
            }
        }

        private void SolutionsMessageHandler(SolutionsMessage message)
        {
            throw new NotImplementedException();
        }

        private void ErrorMessageHandler(ErrorMessage message)
        {
            switch (message.ErrorType)
            {
                case ErrorType.UnknownSender:
                    Register();
                    return;
                case ErrorType.InvalidOperation:
                case ErrorType.ExceptionOccured:
                    throw new NotImplementedException();
            }
        }
    }
}
