using System;
using System.Collections.Generic;
using System.Net;
using Dvrp.Ucc.Commons.Components;
using Dvrp.Ucc.Commons.Computations.Base;
using Dvrp.Ucc.Commons.Messaging;
using Dvrp.Ucc.Commons.Messaging.Models;
using Dvrp.Ucc.Commons.Messaging.Models.Base;
using Dvrp.Ucc.TaskSolver;

namespace Dvrp.Ucc.ComputationalNode
{
    /// <summary>
    ///     The computational node class providing communication with server and solving partial problems using task solvers.
    /// </summary>
    public sealed class ComputationalNode : ComputationalComponent
    {
        /// <summary>
        ///     Creates a computational node which looks for task solvers in current directory.
        /// </summary>
        /// <param name="threadManager">The thread manager. Cannot be null.</param>
        /// <param name="serverAddress">The primary server address. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="threadManager"/> or <paramref name="serverAddress"/> is null.</exception>        
        /// <exception cref="Dvrp.Ucc.Commons.Exceptions.TaskSolverLoadingException">Thrown when exception occurred
        /// during loading task solvers.</exception>
        public ComputationalNode(ThreadManager threadManager, IPEndPoint serverAddress)
            : base(threadManager, serverAddress)
        {
        }

        /// <summary>
        ///     Creates a computational node.
        /// </summary>
        /// <param name="threadManager">The thread manager. Cannot be null.</param>
        /// <param name="serverAddress">The primary server address. Cannot be null.</param>
        /// <param name="taskSolversDirectoryRelativePath">The relative path to directory with task solvers. If null current directory will be searched for task solvers.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="threadManager"/> or <paramref name="serverAddress"/> is null.</exception>
        /// <exception cref="Dvrp.Ucc.Commons.Exceptions.TaskSolverLoadingException">Thrown when exception occurred
        /// during loading task solvers.</exception>
        public ComputationalNode(ThreadManager threadManager, IPEndPoint serverAddress,
            string taskSolversDirectoryRelativePath)
            : base(threadManager, serverAddress, taskSolversDirectoryRelativePath)
        {
        }

        /// <summary>
        ///     The type of the component.
        /// </summary>
        public override ComponentType ComponentType => ComponentType.ComputationalNode;

        /// <summary>
        ///     Handles any message received from server after registration process completes successfully.
        /// </summary>
        /// <param name="message">Message to handle.</param>
        /// <remarks>
        ///     RegisterResponse is handled in base class.
        /// </remarks>
        /// <exception cref="System.InvalidOperationException">Thrown when received not supported message type.</exception>
        protected override void HandleReceivedMessage(Message message)
        {
            switch (message.MessageType)
            {
                case MessageClass.NoOperation:
                    NoOperationMessageHandler((NoOperationMessage)message);
                    break;
                case MessageClass.SolvePartialProblems:
                    PartialProblemsMessageHandler((PartialProblemsMessage)message);
                    break;
                case MessageClass.Error:
                    ErrorMessageHandler((ErrorMessage)message);
                    break;
                default:
                    throw new InvalidOperationException("Received not supported message type.");
            }
        }

        /// <summary>
        /// Handler for NoOperationMessage.
        /// </summary>
        /// <param name="message">A NoOperationMessage.</param>
        private void NoOperationMessageHandler(NoOperationMessage message)
        {
            // nothing to do, backuping is handled by MessageSender
        }

        /// <summary>
        /// Handler for PartialProblemsMessage.
        /// </summary>
        /// <param name="message">A PartialProblemsMessage.</param>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown when:
        ///     - problem type can't be solved with this ComputationalNode,
        ///     - received more partial problems than can be currently started.
        /// </exception>
        private void PartialProblemsMessageHandler(PartialProblemsMessage message)
        {
            if (!TaskSolvers.TryGetValue(message.ProblemType, out var taskSolverType))
            {
                // shouldn't ever get here - received unsolvable problem
                throw new InvalidOperationException(
                    $"\"{message.ProblemType}\" problem type can't be solved with this ComputationalNode.");
            }
            var timeout = message.SolvingTimeout.HasValue
                ? TimeSpan.FromMilliseconds(message.SolvingTimeout.Value)
                : TimeSpan.MaxValue;
            foreach (var partialProblem in message.PartialProblems)
            {
                /* each partial problem should be started properly cause server sends at most 
                 * as many partial problems as count of component's tasks in idle state */
                var actionDescription =
                    $"Solving partial problem \"{message.ProblemType}\"(problem instance id={message.ProblemInstanceId})(partial problem id={partialProblem.PartialProblemId})";
                var started = StartActionInNewThread(() =>
                {
                    // not sure if TaskSolver can change CommonData during computations so recreate it for each partial problem
                    var taskSolver = (TaskSolver.TaskSolver?)Activator.CreateInstance(taskSolverType, message.CommonData) ?? throw new ArgumentNullException(nameof(taskSolverType));
                    taskSolver.ThrowIfError();

                    // measure time using DateTime cause StopWatch is not guaranteed to be thread safe
                    var start = DateTime.UtcNow;
                    var partialProblemSolutionData = taskSolver.Solve(partialProblem.Data, timeout);
                    var stop = DateTime.UtcNow;

                    taskSolver.ThrowIfError();

                    var solutions = new List<SolutionsMessage.Solution>
                    {
                        new ()
                        {
                            PartialProblemId = partialProblem.PartialProblemId,
                            TimeoutOccured = taskSolver.State == TaskSolverState.Timeout,
                            Type = SolutionsMessage.SolutionType.Partial,
                            ComputationsTime = (ulong) (stop - start).TotalMilliseconds,
                            Data = partialProblemSolutionData
                        }
                    };
                    var solutionsMessage = new SolutionsMessage(message.ProblemType)
                    {
                        ProblemInstanceId = message.ProblemInstanceId,
                        CommonData = message.CommonData,
                        Solutions = solutions
                    };

                    EnqueueMessageToSend(solutionsMessage);
                }, actionDescription, message.ProblemType, message.ProblemInstanceId, partialProblem.PartialProblemId);
                if (!started)
                {
                    throw new InvalidOperationException("Received more partial problems than can be currently started.");
                }
            }
        }

        /// <summary>
        /// Handler for ErrorMessage.
        /// </summary>
        /// <param name="message">An ErrorMessage.</param>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown when <paramref name="message"/> received from server contains information about server exception.
        /// </exception>
        private void ErrorMessageHandler(ErrorMessage message)
        {
            switch (message.ErrorType)
            {
                case ErrorType.UnknownSender:
                    Register();
                    break;
                case ErrorType.InvalidOperation:
                    // nothing to do
                    break;
                case ErrorType.ExceptionOccured:
                    throw new InvalidOperationException(
                        "Information about exception on server shouldn't be send to component.");
            }
        }
    }
}