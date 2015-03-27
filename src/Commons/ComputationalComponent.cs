using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using _15pl04.Ucc.Commons.Messaging.Models;
using UCCTaskSolver;

namespace _15pl04.Ucc.Commons
{
    /// <summary>
    /// Base class for ComputationalNode and TaskManager.
    /// </summary>
    public abstract class ComputationalComponent
    {
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
        /// The address of Communication Server.
        /// </summary>
        protected readonly IPEndPoint _serverAddress;

        /// <summary>
        /// The dictionary of TaskSolvers; the keys are names of problems.
        /// </summary>
        protected ReadOnlyDictionary<string, TaskSolver> TaskSolvers { get; private set; }

        /// <summary>
        /// The task pool that provides starting computations in tasks.
        /// </summary>
        protected ComputationalTaskPool ComputationalTaskPool { get; private set; }


        private MessageSender _messageSender;

        private const string TaskSolversDirectory = "TaskSolvers";

        private Task _messagingTask;
        private CancellationTokenSource _cancellationTokenSource;

        private ConcurrentQueue<Message> _messagesToSend;
        private ManualResetEvent _messagesToSendManualResetEvent;


        /// <summary>
        /// Creates component that can register to the server.
        /// </summary>
        /// <param name="serverAddress">Communication server address.</param>
        public ComputationalComponent(IPEndPoint serverAddress)
        {
            _serverAddress = serverAddress;
            _messageSender = new MessageSender(_serverAddress);

            TaskSolvers = GetTaskSolvers();

            _messagesToSend = new ConcurrentQueue<Message>();
            _messagesToSendManualResetEvent = new ManualResetEvent(false);

            // information for registration message; probably it is a temporary solution
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
            // get RegisterMessage
            var registerMessage = GetRegisterMessage();
            // send it and get response
            var responseMessages = _messageSender.Send(registerMessage);
            // and try to save received information
            try
            {
                var registerResponseMessage = (RegisterResponseMessage)responseMessages.FirstOrDefault(m => m.MessageType == Message.MessageClassType.RegisterResponse);
                ID = registerResponseMessage.Id;
                Timeout = registerResponseMessage.Timeout;
            }
            catch (Exception ex)
            {
                throw new Commons.Exceptions.RegisterException("Couldn't register component to server.", ex);
            }

            ComputationalTaskPool = new ComputationalTaskPool(_parallelThreads, _cancellationTokenSource.Token);

            // start informing about status(es) of threads
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
        /// Handles a message received from server.
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
        }


        private void HandleResponseMessages(Message[] messages)
        {
            foreach (var message in messages)
            {
                HandleResponseMessage(message);
            }
        }

        /// <summary>
        /// Gets dictionary with names of solvable problems as keys and proper TaskSolvers as values.
        /// </summary>
        /// <returns>A dictionary with names of solvable problems as keys and proper TaskSolvers as values.</returns>
        private ReadOnlyDictionary<string, TaskSolver> GetTaskSolvers()
        {
            var dictionary = new Dictionary<string, TaskSolver>();

            // add <key,value> pairs based on types derived from TaskSolver in *.dll files
            var taskSolversDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), TaskSolversDirectory);
            var libraries = Directory.GetFiles(taskSolversDirectoryPath, "*.dll");
            var typeOfTaskSolver = typeof(TaskSolver);
            foreach (var file in libraries)
            {
                Assembly assembly = Assembly.LoadFile(file);
                var taskSolversTypes = assembly.GetTypes().Where(t => typeOfTaskSolver.IsAssignableFrom(t) && !t.IsAbstract);
                foreach (var taskSolverType in taskSolversTypes)
                {
                    var taskSolver = (TaskSolver)Activator.CreateInstance(taskSolverType);
                    dictionary.Add(taskSolver.Name, taskSolver);
                }
            }

            var readOnlyDictionary = new ReadOnlyDictionary<string, TaskSolver>(dictionary);
            return readOnlyDictionary;
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
                Message[] responseMessages;
                while (true)
                {
                    while (_messagesToSend.TryDequeue(out message))
                    {
                        // send message
                        responseMessages = _messageSender.Send(message);
                        if (responseMessages == null)
                            throw new Commons.Exceptions.NoResponseException();
                        // and handle response
                        HandleResponseMessages(responseMessages);
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
                        responseMessages = _messageSender.Send(statusMessage);
                        if (responseMessages == null)
                            throw new Commons.Exceptions.NoResponseException();
                        // and handle response
                        HandleResponseMessages(responseMessages);
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
            // to delete after uncommenting
            throw new NotImplementedException();

            var threadsStatuses = new List<StatusThread>();
            foreach (var computationalTask in ComputationalTaskPool.ComputationalTasks)
            {
                var threadStatus = new StatusThread()
                {
                    // to uncomment after StatusThread will be fixed
                    //ProblemType=computationalTask.ProblemType,
                    //ProblemInstanceId=computationalTask.ProblemInstanceId,
                    //TaskId=computationalTask.PartialProblemId,
                    State = computationalTask.State,
                    HowLong = (ulong)computationalTask.TimeSinceLastStateChange.TotalMilliseconds
                };
                threadsStatuses.Add(threadStatus);
            }
            StatusMessage statusMessage = new StatusMessage()
            {
                Id = ID,
                Threads = threadsStatuses
            };
            return statusMessage;
        }
    }
}
