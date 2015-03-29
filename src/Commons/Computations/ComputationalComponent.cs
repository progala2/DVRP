using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons.Computations
{
    /// <summary>
    /// Base class for ComputationalNode and TaskManager.
    /// </summary>
    public abstract class ComputationalComponent
    {
        public event MessageEventHandler MessageEnqueued;
        public event MessageEventHandler MessageSended;
        public event MessageEventHandler MessageReceived;

        public event MessageHandlingExceptionHandler MessageHandlingException;

        /// <summary>
        /// The ID assigned by the Communication Server.
        /// </summary>
        public ulong ID { get; private set; }

        /// <summary>
        /// The communication timeout configured on Communication Server.
        /// </summary>
        public uint Timeout { get; private set; }

        /// <summary>
        /// The number of threads that could be efficiently run in parallel.
        /// </summary>
        protected readonly byte _parallelThreads;

        /// <summary>
        /// The dictionary of TaskSolvers types; the keys are names of problems.
        /// </summary>
        protected ReadOnlyDictionary<string, Type> TaskSolvers { get; private set; }

        /// <summary>
        /// The task pool that provides starting computations in tasks.
        /// </summary>
        protected ComputationalTaskPool ComputationalTaskPool { get; private set; }

        private MessageSender _messageSender;

        private Task _messagingTask;
        private CancellationTokenSource _cancellationTokenSource;

        private ConcurrentQueue<Message> _messagesToSend;
        private ManualResetEvent _messagesToSendManualResetEvent;


        /// <summary>
        /// Creates component that can register to the server.
        /// </summary>
        /// <param name="serverAddress">Communication server address.</param>
        public ComputationalComponent(IPEndPoint serverAddress)
            : this(serverAddress, null)
        {
        }

        /// <summary>
        /// Creates component that can register to the server.
        /// </summary>
        /// <param name="serverAddress">Communication server address.</param>
        /// <param name="taskSolversDirectoryRelativePath">Relative path to directory containging task solvers libraries.</param>
        public ComputationalComponent(IPEndPoint serverAddress, string taskSolversDirectoryRelativePath)
        {
            TaskSolvers = TaskSolversLoader.GetTaskSolversFromRelativePath(taskSolversDirectoryRelativePath);

            _messageSender = new MessageSender(serverAddress);

            _messagesToSend = new ConcurrentQueue<Message>();
            _messagesToSendManualResetEvent = new ManualResetEvent(false);

            // information for registration message; probably it could be changed
            _parallelThreads = (byte)Environment.ProcessorCount;

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.Token.Register(() =>
            {
                _messagingTask = null;
                ComputationalTaskPool = null;
            });
        }


        /// <summary>
        /// Registers component to server and starts work.
        /// </summary>
        /// <exception cref="_15pl04.Ucc.Commons.Exceptions.RegisterException"></exception>
        public void Start()
        {
            try
            {
                Register();
            }
            catch (Commons.Exceptions.RegisterException)
            {
                throw;
            }

            ComputationalTaskPool = new ComputationalTaskPool(_parallelThreads, _cancellationTokenSource.Token);

            // start informing about statuses of threads
            _messagingTask = new Task(() => MessagesProcessing(), _cancellationTokenSource.Token);
            _messagingTask.Start();
        }

        /// <summary>
        /// Stops component work.
        /// </summary>
        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }


        /// <summary>
        /// Gets RegisterMessage specified for this component.
        /// </summary>
        /// <returns>A proper RegisterMessage.</returns>
        protected abstract RegisterMessage GetRegisterMessage();

        /// <summary>
        /// Handles each message received from server after registration is completed.
        /// So it does not handle RegisterResponseMessage.
        /// </summary>
        /// <param name="message">A message to handle. It is received from server.</param>
        /// <remarks>
        /// Here can be started computational tasks using ComputationalTaskPool property.
        /// </remarks>
        protected abstract void HandleResponseMessage(Message message);


        /// <summary>
        /// Enqueues message to be send to server.
        /// </summary>
        /// <param name="message">A message to send.</param>
        protected void EnqueueMessageToSend(Message message)
        {
            _messagesToSend.Enqueue(message);
            _messagesToSendManualResetEvent.Set();
            RaiseMessageEvent(MessageEnqueued, message);
        }

        /// <exception cref="_15pl04.Ucc.Commons.Exceptions.RegisterException"></exception>
        private void Register()
        {
            // send RegisterMessage and get response
            var registerMessage = GetRegisterMessage();
            Message[] receivedMessages;
            try
            {
                receivedMessages = SendMessage(registerMessage);
            }
            catch (Commons.Exceptions.NoResponseException ex)
            {
                throw new Commons.Exceptions.RegisterException("Couldn't register component to server.", ex);
            }
            // and try to save received information
            bool registered = false;
            foreach (var receivedMessage in receivedMessages)
            {
                RegisterResponseMessage registerResponseMessage;
                if ((registerResponseMessage = receivedMessage as RegisterResponseMessage) != null)
                {
                    ID = registerResponseMessage.Id;
                    Timeout = registerResponseMessage.Timeout;
                    registered = true;
                }
                else
                {
                    if (registered)
                    {
                        InternalHandleResponseMessage(receivedMessage);
                    }
                    else
                    {
                        // shouldn't ever happen
                        var errorMessage = new ErrorMessage()
                        {
                            ErrorMessageType = ErrorMessageErrorType.ExceptionOccured,
                            ErrorMessageText = "Received " + receivedMessage.MessageType + "Message before RegisterResponseMessage."
                        };
                        EnqueueMessageToSend(errorMessage);
                    }
                }
            }
            if (!registered)
                throw new Commons.Exceptions.RegisterException("Couldn't register component to server.");
        }

        /// <summary>
        /// Message processing loop.
        /// </summary>
        /// <exception cref="_15pl04.Ucc.Commons.Exceptions.NoResponseException"></exception>
        private void MessagesProcessing()
        {
            try
            {
                Message message;
                Message[] receivedMessages;
                while (true)
                {
                    while (_messagesToSend.TryDequeue(out message))
                    {
                        // send message
                        receivedMessages = SendMessageAndStopIfNoResponse(message);
                        // and handle response
                        HandleResponseMessages(receivedMessages);
                    }
                    // no more messages to send so reset state to nonsignaled
                    _messagesToSendManualResetEvent.Reset();

                    // wait some time for new messages to be enqueued to send (_timeout / 2 is just a proposition)
                    var anyNewMessageEnqueuedToSend = _messagesToSendManualResetEvent.WaitOne((int)(Timeout / 2));

                    // if there are no new messages enqueued to send 
                    if (!anyNewMessageEnqueuedToSend)
                    {
                        // send status message
                        var statusMessage = GetStatusMessage();
                        receivedMessages = SendMessageAndStopIfNoResponse(statusMessage);
                        // and handle response
                        HandleResponseMessages(receivedMessages);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // component stops... nothing to do
            }
        }

        /// <summary>
        /// Gets status of this component.
        /// </summary>
        /// <returns>Proper StatusMessage.</returns>
        private StatusMessage GetStatusMessage()
        {
            var threadsStatuses = new List<ThreadStatus>();
            foreach (var computationalTask in ComputationalTaskPool.ComputationalTasks)
            {
                var threadStatus = new ThreadStatus()
                {
                    ProblemType = computationalTask.ProblemType,
                    ProblemInstanceId = computationalTask.ProblemInstanceId,
                    TaskId = computationalTask.PartialProblemId,
                    State = computationalTask.State,
                    HowLong = (ulong)computationalTask.TimeSinceLastStateChange.TotalMilliseconds
                };
                threadsStatuses.Add(threadStatus);
            }
            var statusMessage = new StatusMessage()
            {
                Id = ID,
                Threads = threadsStatuses
            };
            return statusMessage;
        }

        private void HandleResponseMessages(Message[] messages)
        {
            foreach (var message in messages)
            {
                InternalHandleResponseMessage(message);
            }
        }

        private void InternalHandleResponseMessage(Message message)
        {
            try
            {
                HandleResponseMessage(message);
            }
            catch (Exception ex)
            {
                if (MessageHandlingException != null)
                    MessageHandlingException(this, new MessageHandlingExceptionEventArgs(message, ex));
            }
        }

        private Message[] SendMessageAndStopIfNoResponse(Message message)
        {
            try
            {
                var receivedMessages = SendMessage(message);
                return receivedMessages;
            }
            catch (Commons.Exceptions.NoResponseException)
            {
                Stop();
                return null;
            }
        }

        /// <exception cref="_15pl04.Ucc.Commons.Exceptions.NoResponseException"></exception>
        private Message[] SendMessage(Message message)
        {
            var receivedMessages = _messageSender.Send(message);
            RaiseMessageEvent(MessageSended, message);
            if (receivedMessages == null)
                throw new Commons.Exceptions.NoResponseException();
            foreach (var receivedMessage in receivedMessages)
            {
                RaiseMessageEvent(MessageReceived, receivedMessage);
            }
            return receivedMessages;
        }

        private void RaiseMessageEvent(MessageEventHandler messageEventHandler, Message message)
        {
            if (messageEventHandler != null)
            {
                messageEventHandler(this, new MessageEventArgs(message));
            }
        }
    }
}
