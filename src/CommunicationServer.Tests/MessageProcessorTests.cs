using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.Commons.Messaging.Marshalling;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;
using _15pl04.Ucc.CommunicationServer.Messaging;

namespace _15pl04.Ucc.CommunicationServer.Tests
{
    [TestClass]
    public class MessageProcessorTests
    {
        private readonly Marshaller _marshaller;
        private MessageProcessor _processor;

        public MessageProcessorTests()
        {
            var validator = new MessageValidator();
            var serializer = new MessageSerializer();
            _marshaller = new Marshaller(serializer, validator);

            //_processor = new MessageProcessor(_marshaller, 10000);
        }

        [TestMethod]
        public void RegistrationMessageIsAcceptedAndItsCallbackInvoked()
        {
            var waitHandle = new AutoResetEvent(false);
            var callbackCalled = false;

            var msg = new RegisterMessage
            {
                ParallelThreads = 5,
                SolvableProblems = new List<string> {"dvrp"},
                ComponentType = ComponentType.ComputationalNode
            };

            var rawMsg = _marshaller.Marshall(new Message[] {msg});
            TcpServer.ResponseCallback c = (byte[] r) =>
            {
                callbackCalled = true;
                waitHandle.Set();
            };
            //_processor.EnqueueDataToProcess(rawMsg, null,c);

            waitHandle.WaitOne(5000);

            Assert.IsTrue(callbackCalled);
        }

        [TestMethod]
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

            Message outputMessage = null;
            TcpServer.ResponseCallback c = (byte[] r) =>
            {
                outputMessage = _marshaller.Unmarshall(r)[0];
                waitHandle.Set();
            };
            //_processor.EnqeueInputMessage(rawMsg, c);

            waitHandle.WaitOne(10000);

            Assert.IsInstanceOfType(outputMessage, typeof (RegisterResponseMessage));
        }

        [TestMethod]
        public void SolveRequestMessageIsAcceptedAndSolveRequestResponseMessageIsPassedBack()
        {
            var waitHandle = new AutoResetEvent(false);

            var msg = new SolveRequestMessage
            {
                ProblemData = new byte[0],
                ProblemType = "dvrp"
            };
            var rawMsg = _marshaller.Marshall(new Message[] {msg});

            Message outputMessage = null;
            TcpServer.ResponseCallback c = r =>
            {
                outputMessage = _marshaller.Unmarshall(r)[0];
                waitHandle.Set();
            };
            //_processor.EnqeueInputMessage(rawMsg, c);

            waitHandle.WaitOne(5000);

            Assert.IsInstanceOfType(outputMessage, typeof (SolveRequestResponseMessage));
        }

        [TestMethod]
        public void StatusMessageFromAnUnregisteredComponentReturnsErrorMessage()
        {
            var waitHandle = new AutoResetEvent(false);

            var msg = new StatusMessage
            {
                ComponentId = 5,
                Threads = new List<ThreadStatus> {new ThreadStatus {ProblemType = "dvrp"}}
            };
            var rawMsg = _marshaller.Marshall(new Message[] {msg});

            Message outputMessage = null;
            TcpServer.ResponseCallback c = (byte[] r) =>
            {
                outputMessage = _marshaller.Unmarshall(r)[0];
                waitHandle.Set();
            };
            //_processor.EnqeueInputMessage(rawMsg, c);

            waitHandle.WaitOne(5000);

            Assert.IsInstanceOfType(outputMessage, typeof (ErrorMessage));
        }

        [TestMethod]
        public void StatusMessageFromARegisteredComponentReturnsNoOperationIfThereAreNoTasksPending()
        {
            //ComponentMonitor.Instance.RegisterNode(Commons.ComponentType.TaskManager, 1, new string[]{"dvrp"});

            var waitHandle = new AutoResetEvent(false);

            var msg = new StatusMessage
            {
                ComponentId = 5,
                Threads = new List<ThreadStatus> {new ThreadStatus {ProblemType = "dvrp"}}
            };
            var rawMsg = _marshaller.Marshall(new Message[] {msg});

            Message outputMessage = null;
            TcpServer.ResponseCallback c = (byte[] r) =>
            {
                outputMessage = _marshaller.Unmarshall(r)[0];
                waitHandle.Set();
            };
            //_processor.EnqeueInputMessage(rawMsg, c);

            waitHandle.WaitOne(5000);

            Assert.IsInstanceOfType(outputMessage, typeof (ErrorMessage));
        }
    }
}