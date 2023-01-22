using System;
using System.Collections.Generic;
using System.Threading;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.Commons.Messaging.Marshalling;
using _15pl04.Ucc.Commons.Messaging.Marshalling.Base;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;
using _15pl04.Ucc.CommunicationServer.Components;
using _15pl04.Ucc.CommunicationServer.Components.Base;
using _15pl04.Ucc.CommunicationServer.Messaging;
using _15pl04.Ucc.CommunicationServer.Messaging.Base;
using _15pl04.Ucc.CommunicationServer.WorkManagement;
using _15pl04.Ucc.CommunicationServer.WorkManagement.Base;
using Xunit;
using ErrorMessage = _15pl04.Ucc.Commons.Messaging.Models.ErrorMessage;

namespace _15pl04.Ucc.CommunicationServer.Tests
{
    public class MessageProcessorTests
    {
        private readonly IMarshaller<Message> _marshaller;
        private readonly IComponentOverseer _overseer;
        private readonly MessageProcessor _processor;
        private readonly IWorkManager _workManager;

        public MessageProcessorTests()
        {
            var serializer = new MessageSerializer();
            var validator = new MessageValidator();
            _marshaller = new Marshaller(serializer, validator);

            _overseer = new ComponentOverseer(5, 1);
            _workManager = new WorkManager(_overseer);

            _processor = new MessageProcessor(_overseer, _workManager);
        }

        [Fact]
        public void RegistrationMessageIsAcceptedAndItsCallbackInvoked()
        {
            var waitHandle = new AutoResetEvent(false);
            var callbackCalled = false;

            var msg = new RegisterMessage
            {
                ComponentType = ComponentType.TaskManager,
                ParallelThreads = 5,
                SolvableProblems = new List<string> {"dvrp"}
            };

            var rawMsg = _marshaller.Marshall(new Message[] {msg});

            ProcessedDataCallback c = response =>
            {
                callbackCalled = true;
                waitHandle.Set();
            };

            _processor.StartProcessing();
            _processor.EnqueueDataToProcess(rawMsg, new Metadata(), c);

            waitHandle.WaitOne(5000);
            _processor.StopProcessing();

            Assert.True(callbackCalled);
        }

        [Fact]
        public void RegistrationMessageIsAcceptedAndRegisterResponseMessageIsPassedBack()
        {
            var waitHandle = new AutoResetEvent(false);

            var msg = new RegisterMessage
            {
                ParallelThreads = 5,
                SolvableProblems = new List<string> {"dvrp"},
                ComponentType = ComponentType.ComputationalNode
            };
            var rawMsg = _marshaller.Marshall(new Message[] {msg});

            Message? outputMessage = null;
            ProcessedDataCallback c = response =>
            {
                outputMessage = _marshaller.Unmarshall(response)[0];
                waitHandle.Set();
            };

            _processor.StartProcessing();
            _processor.EnqueueDataToProcess(rawMsg, new Metadata(), c);

            waitHandle.WaitOne(10000);
            _processor.StopProcessing();

            Assert.IsType<RegisterResponseMessage>(outputMessage);
        }

        [Fact]
        public void SolveRequestMessageIsAcceptedAndSolveRequestResponseMessageIsPassedBack()
        {
            var waitHandle = new AutoResetEvent(false);

            var msg = new SolveRequestMessage
            {
                ProblemData = Array.Empty<byte>(),
                ProblemType = "dvrp"
            };
            var rawMsg = _marshaller.Marshall(new Message[] {msg});

            Message? outputMessage = null;
            ProcessedDataCallback c = response =>
            {
                outputMessage = _marshaller.Unmarshall(response)[0];
                waitHandle.Set();
            };

            _processor.StartProcessing();
            _processor.EnqueueDataToProcess(rawMsg, new Metadata(), c);

            waitHandle.WaitOne(5000);
            _processor.StopProcessing();

            Assert.IsType<SolveRequestResponseMessage>(outputMessage);
        }

        [Fact]
        public void StatusMessageFromAnUnregisteredComponentReturnsErrorMessage()
        {
            var waitHandle = new AutoResetEvent(false);

            var msg = new StatusMessage
            {
                ComponentId = 5,
                Threads = new List<ThreadStatus> {new ThreadStatus {ProblemType = "dvrp"}}
            };
            var rawMsg = _marshaller.Marshall(new Message[] {msg});

            Message? outputMessage = null;
            ProcessedDataCallback c = response =>
            {
                outputMessage = _marshaller.Unmarshall(response)[0];
                waitHandle.Set();
            };

            _processor.StartProcessing();
            _processor.EnqueueDataToProcess(rawMsg, new Metadata(), c);

            waitHandle.WaitOne(5000);
            _processor.StopProcessing();

            Assert.IsType<ErrorMessage>(outputMessage);
        }

        [Fact]
        public void StatusMessageFromARegisteredComponentReturnsNoOperationIfThereAreNoTasksPending()
        {
            var waitHandle = new AutoResetEvent(false);

            var msg = new StatusMessage
            {
                ComponentId = 5,
                Threads = new List<ThreadStatus> {new ThreadStatus {ProblemType = "dvrp"}}
            };
            var rawMsg = _marshaller.Marshall(new Message[] {msg});

            Message? outputMessage = null;
            ProcessedDataCallback c = response =>
            {
                outputMessage = _marshaller.Unmarshall(response)[0];
                waitHandle.Set();
            };
            _processor.StartProcessing();
            _processor.EnqueueDataToProcess(rawMsg, new Metadata(), c);

            waitHandle.WaitOne(5000);
            _processor.StopProcessing();

            Assert.IsType<ErrorMessage>(outputMessage);
        }
    }
}