﻿using System;
using System.Collections.Generic;
using System.Net;
using Dvrp.Ucc.Commons.Exceptions;
using Dvrp.Ucc.Commons.Messaging;
using Dvrp.Ucc.Commons.Messaging.Models;
using Dvrp.Ucc.Commons.Messaging.Models.Base;

namespace Dvrp.Ucc.ComputationalClient
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
        public ComputationalClient(IPEndPoint serverAddress)
        {
	        _messageSender = new MessageSender(serverAddress);
        }

        /// <summary>
        ///     Event which is raised after successful sending message to the server.
        /// </summary>
        public event EventHandler<MessageEventArgs>? MessageSent;

        /// <summary>
        ///     Event which is raised after receiving message from the server.
        /// </summary>
        public event EventHandler<MessageEventArgs>? MessageReceived;

        /// <summary>
        ///     Event which is raised if exception occurred during sending message to the server.
        /// </summary>
        public event EventHandler<MessageExceptionEventArgs>? MessageSendingException;

        /// <summary>
        ///     Event which is raised if exception occurred during handling message received from the server.
        /// </summary>
        public event EventHandler<MessageExceptionEventArgs>? MessageHandlingException;

        /// <summary>
        ///     Sends request for solving the problem.
        /// </summary>
        /// <param name="problemType">The name of the type as given by TaskSolver.</param>
        /// <param name="data">The serialized problem data.</param>
        /// <param name="solvingTimeout">The optional time restriction for solving the problem (in ms).</param>
        /// <returns>The ID of the problem instance assigned by the server or null if server is not responding.</returns>
        public ulong? SendSolveRequest(string problemType, byte[] data, ulong? solvingTimeout)
        {
            var solveRequestMessage = new SolveRequestMessage(problemType, data, solvingTimeout);
            var receivedMessages = SendMessage(solveRequestMessage);
            if (receivedMessages == null)
                return null;

            ulong? problemId = null;
            foreach (var receivedMessage in receivedMessages)
            {
                if (receivedMessage is SolveRequestResponseMessage solveRequestResponseMessage)
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
        public List<SolutionsMessage>? SendSolutionRequest(ulong id)
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
                if (receivedMessage is SolutionsMessage solutionsMessage)
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

        /// <summary>
        /// Sends message to server and gets response.
        /// </summary>
        /// <param name="message">Message to be send to server.</param>
        /// <returns>Received messages or null if couldn't get server response.</returns>
        private Message[]? SendMessage(Message message)
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

        /// <summary>
        /// Raises event handler if it is not null.
        /// </summary>
        /// <param name="eventHandler">Handler to be raised.</param>
        /// <param name="message">Message to be passed as event event argument.</param>
        private void RaiseEvent(EventHandler<MessageEventArgs>? eventHandler, Message message)
        {
            eventHandler?.Invoke(this, new MessageEventArgs(message));
        }

        /// <summary>
        /// Raises event handler if it is not null.
        /// </summary>
        /// <param name="eventHandler">Handler to be raised.</param>
        /// <param name="message">Message to be passed as event event argument.</param>
        /// <param name="exception">Exception to be passed as event event argument.</param>
        private void RaiseEvent(EventHandler<MessageExceptionEventArgs>? eventHandler, Message message,
            Exception exception)
        {
            eventHandler?.Invoke(this, new MessageExceptionEventArgs(message, exception));
        }
    }
}