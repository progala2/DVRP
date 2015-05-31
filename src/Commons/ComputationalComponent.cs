using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.Commons.Computations.Base;
using _15pl04.Ucc.Commons.Exceptions;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.Commons
{
    /// <summary>
    /// Base class of the Computational Node and Task Manager.
    /// </summary>
    public abstract class ComputationalComponent
    {
        private readonly MessageSender _messageSender;
        private readonly object _startLock = new object();
        private readonly ThreadManager _threadManager;
        private Task _messagesProcessingTask;
        private ConcurrentQueue<Message> _messagesToSend;
        private ManualResetEvent _messagesToSendManualResetEvent;

        /// <summary>
        /// </summary>
        /// <param name="threadManager">Thread manager instance. Cannot be null.</param>
        /// <param name="serverAddress">The Communication Server address. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        protected ComputationalComponent(ThreadManager threadManager, IPEndPoint serverAddress)
            : this(threadManager, serverAddress, null)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="threadManager">Thread manager instance. Cannot be null.</param>
        /// <param name="serverAddress">The Communication Server address. Cannot be null.</param>
        /// <param name="taskSolversDirectoryRelativePath">Relative path to directory containing task solver libraries.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        protected ComputationalComponent(ThreadManager threadManager, IPEndPoint serverAddress,
            string taskSolversDirectoryRelativePath)
        {
            if (threadManager == null) throw new ArgumentNullException("threadManager");
            if (serverAddress == null) throw new ArgumentNullException("serverAddress");

            TaskSolvers = TaskSolverLoader.GetTaskSolversFromRelativePath(taskSolversDirectoryRelativePath);

            _threadManager = threadManager;

            _messageSender = new MessageSender(serverAddress);

            MessageHandlingException += (s, e) =>
            {
                if (e != null)
                    InformServerAboutException(string.Format("Message caused exception: {0}", e.Message), e.Exception);
            };
        }

        /// <summary>
        /// The type of the component.
        /// </summary>
        public abstract ComponentType ComponentType { get; }

        /// <summary>
        /// Component ID assigned by the Communication Server.
        /// </summary>
        public ulong Id { get; private set; }

        /// <summary>
        /// Communication timeout configured on Communication Server (in seconds).
        /// </summary>
        public uint Timeout { get; private set; }

        /// <summary>
        /// Informs whether component is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Dictionary of Task Solver types; keys are problem type names.
        /// </summary>
        public ReadOnlyDictionary<string, Type> TaskSolvers { get; private set; }

        public event EventHandler<MessageEventArgs> MessageEnqueuedToSend;
        public event EventHandler<MessageEventArgs> MessageSent;
        public event EventHandler<MessageEventArgs> MessageReceived;
        public event EventHandler<MessageExceptionEventArgs> MessageHandlingException;
        public event EventHandler<MessageExceptionEventArgs> MessageSendingException;
        public event EventHandler OnStarting;
        public event EventHandler OnStarted;

        /// <summary>
        /// Registers component on the server and starts work.
        /// </summary>
        public void Start()
        {
            lock (_startLock)
            {
                if (IsRunning)
                    return;

                RaiseEvent(OnStarting);

                ResetComponent();

                if (!Register())
                    return;

                // start informing about statuses of threads            
                _messagesProcessingTask.Start();

                RaiseEvent(OnStarted);
            }
        }

        /// <summary>
        /// Handles messages received from the Communication Server.
        /// </summary>
        /// <param name="message">Message to handle.</param>
        protected abstract void HandleReceivedMessage(Message message);

        /// <summary>
        ///     Enqueues message to be send to server.
        /// </summary>
        /// <param name="message">A message to send.</param>
        protected void EnqueueMessageToSend(Message message)
        {
            _messagesToSend.Enqueue(message);
            _messagesToSendManualResetEvent.Set();
            RaiseEvent(MessageEnqueuedToSend, message);
        }

        /// <summary>
        /// Starts executing action if there is an available idle thread. This method gets information needed for status messages.
        /// </summary>
        /// <param name="actionToExecute">An action to be executed in new thread. If null no new thread will be started.</param>
        /// <param name="actionDescription">Information about started action that will be send to server if exception occurs during execution.</param>
        /// <param name="problemType">The name of the type as given by the Task Solver.</param>
        /// <param name="problemInstanceId">The ID of the problem assigned when client connected.</param>
        /// <param name="partialProblemId">The ID of the task within given problem instance.</param>
        /// <returns>True if thread was successfully started; false otherwise.</returns>
        protected bool StartActionInNewThread(Action actionToExecute, string actionDescription, string problemType,
            ulong? problemInstanceId, ulong? partialProblemId)
        {
            var started = _threadManager.StartInNewThread(actionToExecute, exception => InformServerAboutException(
                actionDescription, exception), problemType, problemInstanceId, partialProblemId);
            return started;
        }

        /// <summary>
        /// Registers the component in the Communication Server.
        /// </summary>
        /// <returns>True if successfully registered. False otherwise.</returns>
        protected bool Register()
        {
            // send RegisterMessage and get response
            var registerMessage = GetRegisterMessage();
            var receivedMessages = SendMessage(registerMessage);

            if (receivedMessages == null)
                return false;

            // and try to save received information
            var registered = false;
            foreach (var receivedMessage in receivedMessages)
            {
                RegisterResponseMessage registerResponseMessage;
                if ((registerResponseMessage = receivedMessage as RegisterResponseMessage) != null)
                {
                    Id = registerResponseMessage.AssignedId;
                    Timeout = registerResponseMessage.CommunicationTimeout;
                    registered = true;
                }
                else
                {
                    if (registered)
                    {
                        InternalHandleReceivedMessage(receivedMessage);
                    }
                    else
                    {
                        // shouldn't ever happen
                        RaiseEvent(MessageHandlingException, receivedMessage,
                            new InvalidOperationException("RegisterResponseMessage expected."));
                    }
                }
            }
            return registered;
        }


        private void ResetComponent()
        {
            _messagesToSend = new ConcurrentQueue<Message>();
            _messagesToSendManualResetEvent = new ManualResetEvent(false);

            _messagesProcessingTask = new Task(ProcessMessages);
        }

        /// <summary>
        /// Message processing loop.
        /// </summary>
        private void ProcessMessages()
        {
            IsRunning = true;
            // time in milliseconds
            var timeToWait = (int) (Timeout*1000/2);
            while (IsRunning)
            {
                Message messageToSend = GetStatusMessage();
                if (!ProcessMessage(messageToSend))
                    break;

                _messagesToSendManualResetEvent.Reset();

                if (!_messagesToSend.IsEmpty || _messagesToSendManualResetEvent.WaitOne(timeToWait))
                {
                    // should always be true...
                    if (_messagesToSend.TryDequeue(out messageToSend))
                    {
                        if (!ProcessMessage(messageToSend))
                            break;
                    }
                }
            }
            IsRunning = false;
        }

        private bool ProcessMessage(Message messageToSend)
        {
            var receivedMessages = SendMessage(messageToSend);
            if (receivedMessages == null)
                return false;
            foreach (var receivedMessage in receivedMessages)
            {
                InternalHandleReceivedMessage(receivedMessage);
            }
            return true;
        }

        private List<Message> SendMessage(Message message)
        {
            var receivedMessages = _messageSender.Send(message);
            if (receivedMessages == null)
            {
                var noResponseException = new NoResponseException("Server is not responding.");
                RaiseEvent(MessageSendingException, message, noResponseException);
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

        private void InternalHandleReceivedMessage(Message message)
        {
            try
            {
                HandleReceivedMessage(message);
            }
            catch (Exception ex)
            {
                RaiseEvent(MessageHandlingException, message, ex);
            }
        }

        private void InformServerAboutException(string reasonOfException, Exception exception)
        {
            if (exception == null)
                return;
            var errorText = string.Format("{0}(id={1})|{2}|Exception type: {3}|Exception message: {4}",
                ComponentType, Id, reasonOfException, exception.GetType().FullName, exception.Message);
            var errorMessage = new ErrorMessage
            {
                ErrorType = ErrorType.ExceptionOccured,
                ErrorText = errorText
            };
            EnqueueMessageToSend(errorMessage);
        }

        /// <summary>
        ///     Gets RegisterMessage specified for this component.
        /// </summary>
        /// <returns>A proper RegisterMessage.</returns>
        private RegisterMessage GetRegisterMessage()
        {
            var registerMessage = new RegisterMessage
            {
                ComponentType = ComponentType,
                ParallelThreads = _threadManager.ParallelThreads,
                SolvableProblems = new List<string>(TaskSolvers.Keys)
            };
            return registerMessage;
        }

        /// <summary>
        ///     Gets status of this component.
        /// </summary>
        /// <returns>Proper StatusMessage.</returns>
        private StatusMessage GetStatusMessage()
        {
            var threadsStatuses = new List<ThreadStatus>(_threadManager.ThreadStatuses.Count);
            foreach (var computationalThreadStatus in _threadManager.ThreadStatuses)
            {
                var threadStatus = new ThreadStatus
                {
                    ProblemType = computationalThreadStatus.ProblemType,
                    ProblemInstanceId = computationalThreadStatus.ProblemInstanceId,
                    PartialProblemId = computationalThreadStatus.PartialProblemId,
                    State = computationalThreadStatus.State,
                    TimeInThisState = (ulong) computationalThreadStatus.TimeSinceLastStateChange.TotalMilliseconds
                };
                threadsStatuses.Add(threadStatus);
            }
            var statusMessage = new StatusMessage
            {
                ComponentId = Id,
                Threads = threadsStatuses
            };
            return statusMessage;
        }

        private void RaiseEvent(EventHandler eventHandler)
        {
            if (eventHandler != null)
            {
                eventHandler(this, EventArgs.Empty);
            }
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