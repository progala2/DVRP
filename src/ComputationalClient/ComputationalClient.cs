﻿using System;
using System.Collections.Generic;
using System.Net;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.ComputationalClient
{
    public class ComputationalClient
    {
        public event EventHandler<MessageEventArgs> MessageSent;
        public event EventHandler<MessageEventArgs> MessageReceived;

        public event EventHandler<MessageExceptionEventArgs> MessageSendingException;
        public event EventHandler<MessageExceptionEventArgs> MessageHandlingException;


        private MessageSender _messageSender;


        public ComputationalClient(IPEndPoint serverAddress)
        {
            _messageSender = new MessageSender(serverAddress);
        }


        /// <summary>
        /// Sends request for solving the problem.
        /// </summary>
        /// <param name="problemType">The name of the type as given by TaskSolver.</param>
        /// <param name="data">The serialized problem data.</param>
        /// <param name="solvingTimeout">The optional time restriction for solving the problem (in ms).</param>
        /// <returns>The ID of the problem instance assigned by the server or null if server is not responding.</returns>
        public uint? SendSolveRequest(string problemType, byte[] data, ulong? solvingTimeout)
        {
            var solveRequestMessage = new SolveRequestMessage()
            {
                ProblemType = problemType,
                ProblemData = data,
                SolvingTimeout = solvingTimeout
            };
            var receivedMessages = SendMessage(solveRequestMessage);
            if (receivedMessages == null)
                return null;

            uint? problemId = null;
            SolveRequestResponseMessage solveRequestResponseMessage;
            foreach (var receivedMessage in receivedMessages)
            {
                if ((solveRequestResponseMessage = receivedMessage as SolveRequestResponseMessage) != null)
                {
                    problemId = (uint?)solveRequestResponseMessage.AssignedId;
                }
                else
                {
                    RaiseEvent(MessageHandlingException, receivedMessage, new InvalidOperationException("SolveRequestResponseMessage expected."));
                }
            }
            return problemId;
        }

        /// <summary>
        /// Sends request for solution of the problem.
        /// </summary>
        /// <param name="id">The ID of the problem instance assigned by the server.</param>
        /// <returns>Solutions message(s) or null if server is not responding.</returns>
        public SolutionsMessage[] SendSolutionRequest(uint id)
        {
            var solutionRequestMessage = new SolutionRequestMessage()
            {
                ProblemInstanceId = id
            };
            var receivedMessages = SendMessage(solutionRequestMessage);
            if (receivedMessages == null)
                return null;

            var solutionsMessages = new List<SolutionsMessage>();
            SolutionsMessage solutionsMessage;
            foreach (var receivedMessage in receivedMessages)
            {
                if ((solutionsMessage = receivedMessage as SolutionsMessage) != null)
                {
                    solutionsMessages.Add(solutionsMessage);
                }
                else
                {
                    RaiseEvent(MessageHandlingException, receivedMessage, new InvalidOperationException("SolutionsMessage expected."));
                }
            }
            return solutionsMessages.ToArray();
        }


        private List<Message> SendMessage(Message message)
        {
            var receivedMessages = _messageSender.Send(message);
            if (receivedMessages == null)
            {
                var exception = new Commons.Exceptions.NoResponseException("Server is not responding.");
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

        #region RaiseEvent

        private void RaiseEvent(EventHandler<MessageEventArgs> eventHandler, Message message)
        {
            if (eventHandler != null)
            {
                eventHandler(this, new MessageEventArgs(message));
            }
        }

        private void RaiseEvent(EventHandler<MessageExceptionEventArgs> eventHandler, Message message, Exception exception)
        {
            if (eventHandler != null)
            {
                eventHandler(this, new MessageExceptionEventArgs(message, exception));
            }
        } 

        #endregion
    }
}
