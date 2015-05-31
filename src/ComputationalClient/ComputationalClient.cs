using System;
using System.Collections.Generic;
using System.Net;
using _15pl04.Ucc.Commons.Exceptions;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.ComputationalClient
{
    /// <summary>
    ///     The computational client class providing sending to the communication server either solve request or solution request and receiving server respone.
    /// </summary>
    public class ComputationalClient
    {
        private readonly MessageSender _messageSender;

        /// <summary>
        ///     Creates a computational client.
        /// </summary>
        /// <param name="serverAddress">The primary server address. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when <paramref name="serverAddress"/> is null.</exception>
        public ComputationalClient(IPEndPoint serverAddress)
        {
            if (serverAddress == null)
                throw new ArgumentNullException("serverAddress");

            _messageSender = new MessageSender(serverAddress);
        }

        /// <summary>
        ///     Event which is raised after successful sending message to the server.
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageSent;

        /// <summary>
        ///     Event which is raised after receiving message from the server.
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageReceived;

        /// <summary>
        ///     Event which is raised if exception occured during sending message to the server.
        /// </summary>
        public event EventHandler<MessageExceptionEventArgs> MessageSendingException;

        /// <summary>
        ///     Event which is raised if exception occured during handling message received from the server.
        /// </summary>
        public event EventHandler<MessageExceptionEventArgs> MessageHandlingException;

        /// <summary>
        ///     Sends request for solving the problem.
        /// </summary>
        /// <param name="problemType">The name of the type as given by TaskSolver.</param>
        /// <param name="data">The serialized problem data.</param>
        /// <param name="solvingTimeout">The optional time restriction for solving the problem (in ms).</param>
        /// <returns>The ID of the problem instance assigned by the server or null if server is not responding.</returns>
        public ulong? SendSolveRequest(string problemType, byte[] data, ulong? solvingTimeout)
        {
            var solveRequestMessage = new SolveRequestMessage
            {
                ProblemType = problemType,
                ProblemData = data,
                SolvingTimeout = solvingTimeout
            };
            var receivedMessages = SendMessage(solveRequestMessage);
            if (receivedMessages == null)
                return null;

            ulong? problemId = null;
            foreach (var receivedMessage in receivedMessages)
            {
                SolveRequestResponseMessage solveRequestResponseMessage;
                if ((solveRequestResponseMessage = receivedMessage as SolveRequestResponseMessage) != null)
                {
                    problemId = solveRequestResponseMessage.AssignedId;
                }
                else
                {
                    RaiseEvent(MessageHandlingException, receivedMessage,
                        new InvalidOperationException("SolveRequestResponseMessage expected."));
                }
            }
            return problemId;
        }

        /// <summary>
        ///     Sends request for solution of the problem.
        /// </summary>
        /// <param name="id">The ID of the problem instance assigned by the server.</param>
        /// <returns>Solutions message(s) or null if server is not responding.</returns>
        public List<SolutionsMessage> SendSolutionRequest(ulong id)
        {
            var solutionRequestMessage = new SolutionRequestMessage
            {
                ProblemInstanceId = id
            };
            var receivedMessages = SendMessage(solutionRequestMessage);
            if (receivedMessages == null)
                return null;

            var solutionsMessages = new List<SolutionsMessage>();
            foreach (var receivedMessage in receivedMessages)
            {
                SolutionsMessage solutionsMessage;
                if ((solutionsMessage = receivedMessage as SolutionsMessage) != null)
                {
                    solutionsMessages.Add(solutionsMessage);
                }
                else
                {
                    RaiseEvent(MessageHandlingException, receivedMessage,
                        new InvalidOperationException("SolutionsMessage expected."));
                }
            }
            return solutionsMessages;
        }

        private List<Message> SendMessage(Message message)
        {
            var receivedMessages = _messageSender.Send(message);
            if (receivedMessages == null)
            {
                var exception = new NoResponseException("Server is not responding.");
                RaiseEvent(MessageSendingException, message, exception);
            }
            else
            {
                RaiseEvent(MessageSent, message);
                foreach (var receivedMessage in receivedMessages)
                {
                    RaiseEvent(MessageReceived, receivedMessage);
                }
            }
            return receivedMessages;
        }

        private void RaiseEvent(EventHandler<MessageEventArgs> eventHandler, Message message)
        {
            if (eventHandler != null)
            {
                eventHandler(this, new MessageEventArgs(message));
            }
        }

        private void RaiseEvent(EventHandler<MessageExceptionEventArgs> eventHandler, Message message,
            Exception exception)
        {
            if (eventHandler != null)
            {
                eventHandler(this, new MessageExceptionEventArgs(message, exception));
            }
        }
    }
}