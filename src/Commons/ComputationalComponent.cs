using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using _15pl04.Ucc.Commons.Messaging;
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
        protected readonly Dictionary<string, TaskSolver> _taskSolvers;

        /// <summary>
        /// The ID assigned by the Communication Server.
        /// </summary>
        protected ulong ID { get; private set; }

        /// <summary>
        /// The communication timeout configured on Communication Server.
        /// </summary>
        protected uint Timeout { get; private set; }



        // to change
        protected readonly TcpClient _tcpClient;
        protected readonly Marshaller _marshaller;
        /* it could be a List<IPEndPoint> but messages from server give information about
         * backup servers with List<BackupCommunicationServer> so keeping it this way allows to
         * parse to IPEndPoint only after primary server crash
         */
        protected List<BackupCommunicationServer> _backupCommunicationServers;
        ///////////////////////



        private Task _messagingTask;
        private ComputationalTask[] _computationalTasks;
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
            _tcpClient = new TcpClient(_serverAddress);

            _taskSolvers = GetTaskSolvers();
            _marshaller = new Marshaller();

            _messagesToSend = new ConcurrentQueue<Message>();
            _messagesToSendManualResetEvent = new ManualResetEvent(false);

            // information for registration message; probably it is a temporary solution
            _parallelThreads = (byte)Environment.ProcessorCount;

            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.Token.Register(() =>
            {
                _messagingTask = null;
                if (_computationalTasks != null)
                {
                    for (int i = 0; i < _computationalTasks.Length; i++)
                    {
                        _computationalTasks[i] = null;
                    }
                }
            });
        }

        /// <summary>
        /// Registers component to server and starts work.
        /// </summary>
        public void Start()
        {
            // get RegisterMessage
            var registerMessage = GetRegisterMessage();
            // TRY to send it and get response with _tcpClient.SendData(marshalledMessage);
            throw new NotImplementedException();
            // save information from response
            throw new NotImplementedException();

            _computationalTasks = new ComputationalTask[_parallelThreads];
            for (int i = 0; i < _computationalTasks.Length; i++)
            {
                _computationalTasks[i] = new ComputationalTask();
            }

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
        /// Here should be started computational tasks.
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

        /// <summary>
        /// Starts task if there is an available idle task in pool. This method gets information needed for Status messages.
        /// </summary>
        /// <param name="action">An action to be performed.</param>
        /// <param name="problemType">The name of the type as given by TaskSolver.</param>
        /// <param name="problemInstanceId">The ID of the problem assigned when client connected.</param>
        /// <param name="partialProblemId">The ID of the task within given problem instance.</param>
        /// <returns>Information whether task was successfully started.</returns>
        protected bool StartComputationalTask(Action action, string problemType, ulong? problemInstanceId, ulong? partialProblemId)
        {
            // get index of idle task
            int? idleTaskIndex = GetIdleTaskIndex();
            if (idleTaskIndex == null)
                return false;

            int index = idleTaskIndex.Value;

            // create cancellable task
            var task = new Task(action, _cancellationTokenSource.Token);
            // after task completion reset proper ComputationalTask in array to default values
            task.ContinueWith((t) => _computationalTasks[index] = new ComputationalTask());

            // create ComputationalTask
            _computationalTasks[index] = new ComputationalTask(task)
            {
                ProblemType = problemType,
                ProblemInstanceId = problemInstanceId,
                PartialProblemId = partialProblemId
            };
            // and start it
            _computationalTasks[index].Task.Start();

            return true;
        }

        private int? GetIdleTaskIndex()
        {
            int index = 0;
            while (index < _computationalTasks.Length)
            {
                if (_computationalTasks[index].State == StatusThreadState.Idle)
                    return index;
                index++;
            }
            return null;
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
        private Dictionary<string, TaskSolver> GetTaskSolvers()
        {
            var result = new Dictionary<string, TaskSolver>();

            // add <key,value> pairs based on types derived from TaskSolver in *.dll files
            throw new NotImplementedException();

            return result;
        }

        /// <summary>
        /// Message processing loop.
        /// </summary>
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
                        responseMessages = SendMessage(message);
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
                        responseMessages = SendMessage(statusMessage);
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

        private StatusMessage GetStatusMessage()
        {
            // create StatusMessage based on _id and _computationalTasks fields
            throw new NotImplementedException();
        }

        // probably to delete later
        private Message[] SendMessage(Message message)
        {
            // just because it should be check for possible exceptions
            throw new NotImplementedException();


            var messagesToSend = new Message[] { message };
            var dataToSend = _marshaller.Marshall(messagesToSend);

            var dataReceived = _tcpClient.SendData(dataToSend);

            var messagesReceived = _marshaller.Unmarshall(dataReceived);
            return messagesReceived;
        }
    }
}
