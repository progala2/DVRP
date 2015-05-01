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
    ///     Base class for ComputationalNode and TaskManager.
    /// </summary>
    public abstract class ComputationalComponent
    {
        private readonly MessageSender _messageSender;
        private readonly object _startLock = new object();
        private Task _messagesProcessingTask;
        private ConcurrentQueue<Message> _messagesToSend;
        private ManualResetEvent _messagesToSendManualResetEvent;

        /// <summary>
        ///     Creates component that can register to the server.
        /// </summary>
        /// <param name="threadManager">The thread manager. Cannot be null.</param>
        /// <param name="serverAddress">The communication server address. Cannot be null.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        protected ComputationalComponent(ThreadManager threadManager, IPEndPoint serverAddress)
            : this(threadManager, serverAddress, null)
        {
        }

        /// <summary>
        ///     Creates component that can register to the server.
        /// </summary>
        /// <param name="threadManager">The thread manager. Cannot be null.</param>
        /// <param name="serverAddress">The communication server address. Cannot be null.</param>
        /// <param name="taskSolversDirectoryRelativePath">The relative path to directory containging task solvers libraries.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        protected ComputationalComponent(ThreadManager threadManager, IPEndPoint serverAddress,
            string taskSolversDirectoryRelativePath)
        {
            if (threadManager == null) throw new ArgumentNullException("threadManager");
            if (serverAddress == null) throw new ArgumentNullException("serverAddress");

            TaskSolvers = TaskSolverLoader.GetTaskSolversFromRelativePath(taskSolversDirectoryRelativePath);

            ThreadManager = threadManager;

            _messageSender = new MessageSender(serverAddress);
        }

        /// <summary>
        ///     The type of component.
        /// </summary>
        public abstract ComponentType ComponentType { get; }

        /// <summary>
        ///     The ID assigned by the Communication Server.
        /// </summary>
        public ulong ID { get; private set; }

        /// <summary>
        ///     The communication timeout configured on Communication Server.
        /// </summary>
        public uint Timeout { get; private set; }

        /// <summary>
        ///     Informs whether component is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        ///     The dictionary of TaskSolvers types; the keys are names of problems.
        /// </summary>
        public ReadOnlyDictionary<string, Type> TaskSolvers { get; private set; }

        /// <summary>
        ///     The task pool that provides starting computations in tasks.
        /// </summary>
        protected ThreadManager ThreadManager { get; private set; }

        public event EventHandler<MessageEventArgs> MessageEnqueuedToSend;
        public event EventHandler<MessageEventArgs> MessageSent;
        public event EventHandler<MessageEventArgs> MessageReceived;
        public event EventHandler<MessageExceptionEventArgs> MessageHandlingException;
        public event EventHandler<MessageExceptionEventArgs> MessageSendingException;
        public event EventHandler OnStarting;
        public event EventHandler OnStarted;

        /// <summary>
        ///     Registers component to server and starts work.
        /// </summary>
        public void Start()
        {
            lock (_startLock)
            {
                if (IsRunning)
                {
                    return;
                }

                RaiseEvent(OnStarting);

                ResetComponent();

                if (!Register())
                {
                    return;
                }

                // start informing about statuses of threads            
                _messagesProcessingTask.Start();

                RaiseEvent(OnStarted);
            }
        }

        /// <summary>
        ///     Handles each message received from server after registration is completed.
        ///     So it does not handle RegisterResponseMessage.
        /// </summary>
        /// <param name="message">A message to handle. It is received from server.</param>
        /// <remarks>
        ///     Here can be started computational tasks using ComputationalTaskPool property.
        /// </remarks>
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

        protected bool Register()
        {
            // send RegisterMessage and get response
            var registerMessage = GetRegisterMessage();
            var receivedMessages = SendMessage(registerMessage);

            if (receivedMessages == null)
                return false;

            // and try to save received information
            var registered = false;
            RegisterResponseMessage registerResponseMessage;
            foreach (var receivedMessage in receivedMessages)
            {
                if ((registerResponseMessage = receivedMessage as RegisterResponseMessage) != null)
                {
                    ID = registerResponseMessage.AssignedId;
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

            _messagesProcessingTask = new Task(() => ProcessMessages());
        }

        /// <summary>
        ///     Message processing loop.
        /// </summary>
        private void ProcessMessages()
        {
            IsRunning = true;
            Message messageToSend;
            var timeToWait = (int) (Timeout/2);
            while (IsRunning)
            {
                messageToSend = GetStatusMessage();
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

        /// <summary>
        ///     Gets RegisterMessage specified for this component.
        /// </summary>
        /// <returns>A proper RegisterMessage.</returns>
        private RegisterMessage GetRegisterMessage()
        {
            var registerMessage = new RegisterMessage
            {
                ComponentType = ComponentType,
                ParallelThreads = ThreadManager.ParallelThreads,
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
            var threadsStatuses = new List<ThreadStatus>(ThreadManager.ThreadStatuses.Count);
            foreach (var computationalTask in ThreadManager.ThreadStatuses)
            {
                var threadStatus = new ThreadStatus
                {
                    ProblemType = computationalTask.ProblemType,
                    ProblemInstanceId = computationalTask.ProblemInstanceId,
                    PartialProblemId = computationalTask.PartialProblemId,
                    State = computationalTask.State,
                    TimeInThisState = (ulong) computationalTask.TimeSinceLastStateChange.TotalMilliseconds
                };
                threadsStatuses.Add(threadStatus);
            }
            var statusMessage = new StatusMessage
            {
                ComponentId = ID,
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