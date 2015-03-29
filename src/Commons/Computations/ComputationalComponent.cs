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
        public event EventHandler<MessageEventArgs> MessageEnqueuedToSend;
        public event EventHandler<MessageEventArgs> MessageSent;
        public event EventHandler<MessageEventArgs> MessageReceived;

        public event EventHandler<MessageExceptionEventArgs> MessageHandlingException;
        public event EventHandler<MessageExceptionEventArgs> MessageSendingException;

        public event EventHandler OnStarting;
        public event EventHandler OnStarted;
        public event EventHandler OnStopping;
        public event EventHandler OnStopped;

        /// <summary>
        /// The ID assigned by the Communication Server.
        /// </summary>
        public ulong ID { get; private set; }

        /// <summary>
        /// The communication timeout configured on Communication Server.
        /// </summary>
        public uint Timeout { get; private set; }

        /// <summary>
        /// Informs whether component is running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// The number of threads that could be efficiently run in parallel.
        /// </summary>
        public byte ParallelThreads { get; private set; }

        /// <summary>
        /// The dictionary of TaskSolvers types; the keys are names of problems.
        /// </summary>
        public ReadOnlyDictionary<string, Type> TaskSolvers { get; private set; }

        /// <summary>
        /// The task pool that provides starting computations in tasks.
        /// </summary>
        protected ComputationalTaskPool ComputationalTaskPool { get; private set; }

        private MessageSender _messageSender;

        private Task _messagesProcessingTask;
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
        /// <exception cref="System.IO.DirectoryNotFoundException"></exception>
        public ComputationalComponent(IPEndPoint serverAddress, string taskSolversDirectoryRelativePath)
        {
            _messageSender = new MessageSender(serverAddress);
            TaskSolvers = TaskSolversLoader.GetTaskSolversFromRelativePath(taskSolversDirectoryRelativePath);

            ParallelThreads = (byte)Environment.ProcessorCount;
            IsRunning = false;
        }


        /// <summary>
        /// Registers component to server and starts work.
        /// </summary>
        public void Start()
        {
            if (IsRunning)
                return;

            RaiseEvent(OnStarting);

            ResetComponent();

            if (!Register())
                return;

            // start informing about statuses of threads            
            _messagesProcessingTask.Start();
            IsRunning = true;

            RaiseEvent(OnStarted);
        }

        /// <summary>
        /// Stops component work.
        /// </summary>
        public void Stop()
        {
            if (IsRunning)
            {
                RaiseEvent(OnStopping);
                _cancellationTokenSource.Cancel();
                IsRunning = false;
                RaiseEvent(OnStopped);
            }
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
            RaiseEvent(MessageEnqueuedToSend, message);
        }

        protected bool Register()
        {
            // send RegisterMessage and get response
            var registerMessage = GetRegisterMessage();
            var receivedMessages = SendMessageAndStopIfNoResponse(registerMessage);

            if (receivedMessages == null)
                return false;

            // and try to save received information
            bool registered = false;
            RegisterResponseMessage registerResponseMessage;
            foreach (var receivedMessage in receivedMessages)
            {
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
                        RaiseEvent(MessageSendingException, receivedMessage, new InvalidOperationException("RegisterResponseMessage expected."));
                    }
                }
            }
            return registered;
        }

        private void ResetComponent()
        {
            _messagesToSend = new ConcurrentQueue<Message>();
            _messagesToSendManualResetEvent = new ManualResetEvent(false);

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.Token.Register(() =>
            {
                _messagesProcessingTask = null;
                ComputationalTaskPool = null;
            });

            ComputationalTaskPool = new ComputationalTaskPool(ParallelThreads, _cancellationTokenSource.Token);
            _messagesProcessingTask = new Task(() => MessagesProcessing(), _cancellationTokenSource.Token);

            IsRunning = false;
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
                    /////////////////////////////
                    /////////////////////////////
                    /////////////////////////////
                    /////////////////////////////
                    /////////////////////////////
                    /////////////////////////////
                    /////////////////////////////
                    /////////////////////////////
                    /////////////////////////////
                    /////////////////////////////
                    //////////////////
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
                RaiseEvent(MessageHandlingException, message, ex);
            }
        }

        private Message[] SendMessageAndStopIfNoResponse(Message message)
        {
            var receivedMessages = _messageSender.Send(message);
            RaiseEvent(MessageSent, message);
            if (receivedMessages == null)
            {
                var exception = new Commons.Exceptions.NoResponseException("Server is not responding.");
                RaiseEvent(MessageSendingException, message, exception);
                Stop();
            }
            else
            {
                foreach (var receivedMessage in receivedMessages)
                {
                    RaiseEvent(MessageReceived, receivedMessage);
                }
            }
            return receivedMessages;
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

        private void RaiseEvent(EventHandler<MessageExceptionEventArgs> eventHandler, Message message, Exception exception)
        {
            if (eventHandler != null)
            {
                eventHandler(this, new MessageExceptionEventArgs(message, exception));
            }
        }
    }
}
