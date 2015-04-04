using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.CommunicationServer.Messaging;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using System.Threading;
using System.Collections.Generic;

namespace _15pl04.Ucc.CommunicationServer.Tests
{
    [TestClass]
    public class MessageProcessorTests
    {
        private MessageProcessor _processor;
        private Marshaller _marshaller;

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
            bool callbackCalled = false;

            var msg = new RegisterMessage()
            {
                ParallelThreads = 5,
                SolvableProblems = new System.Collections.Generic.List<string> { "dvrp" },
                ComponentType = Commons.ComponentType.ComputationalNode,
            };

            byte[] rawMsg = _marshaller.Marshall(new Message[] { msg });
            AsyncTcpServer.ResponseCallback c = (byte[] r) => { callbackCalled = true; waitHandle.Set(); };
            //_processor.EnqueueDataToProcess(rawMsg, null,c);

            waitHandle.WaitOne(5000);

            Assert.IsTrue(callbackCalled);
        }


        [TestMethod]
        public void RegistrationMessageIsAcceptedAndRegisterResponseMessageIsPassedBack()
        {
            var waitHandle = new AutoResetEvent(false);

            var msg = new RegisterMessage()
            {
                ParallelThreads = 5,
                SolvableProblems = new System.Collections.Generic.List<string> { "dvrp" },
                ComponentType = Commons.ComponentType.ComputationalNode,
            };
            byte[] rawMsg = _marshaller.Marshall(new Message[] { msg });

            Message outputMessage = null;
            AsyncTcpServer.ResponseCallback c = (byte[] r) => {
                outputMessage = _marshaller.Unmarshall(r)[0];
                waitHandle.Set(); 
            };
            //_processor.EnqeueInputMessage(rawMsg, c);

            waitHandle.WaitOne(10000);

            Assert.IsInstanceOfType(outputMessage, typeof(RegisterResponseMessage));
        }

        [TestMethod]
        public void SolveRequestMessageIsAcceptedAndSolveRequestResponseMessageIsPassedBack()
        {
            var waitHandle = new AutoResetEvent(false);

            var msg = new SolveRequestMessage()
            {
                ProblemData = new byte[0],
                ProblemType = "dvrp",
            };
            byte[] rawMsg = _marshaller.Marshall(new Message[] { msg });

            Message outputMessage = null;
            AsyncTcpServer.ResponseCallback c = (byte[] r) =>
            {
                outputMessage = _marshaller.Unmarshall(r)[0];
                waitHandle.Set();
            };
            //_processor.EnqeueInputMessage(rawMsg, c);

            waitHandle.WaitOne(5000);

            Assert.IsInstanceOfType(outputMessage, typeof(SolveRequestResponseMessage));
        }

        [TestMethod]
        public void StatusMessageFromAnUnregisteredComponentReturnsErrorMessage()
        {
            var waitHandle = new AutoResetEvent(false);

            var msg = new StatusMessage()
            {
                ComponentId = 5,
                Threads = new List<ThreadStatus>(){new ThreadStatus() { ProblemType = "dvrp"}},                
            };
            byte[] rawMsg = _marshaller.Marshall(new Message[] { msg });

            Message outputMessage = null;
            AsyncTcpServer.ResponseCallback c = (byte[] r) =>
            {
                outputMessage = _marshaller.Unmarshall(r)[0];
                waitHandle.Set();
            };
            //_processor.EnqeueInputMessage(rawMsg, c);

            waitHandle.WaitOne(5000);

            Assert.IsInstanceOfType(outputMessage, typeof(ErrorMessage));
        }

        [TestMethod]
        public void StatusMessageFromARegisteredComponentReturnsNoOperationIfThereAreNoTasksPending()
        {
            //ComponentMonitor.Instance.RegisterNode(Commons.ComponentType.TaskManager, 1, new string[]{"dvrp"});

            var waitHandle = new AutoResetEvent(false);

            var msg = new StatusMessage()
            {
                ComponentId = 5,
                Threads = new List<ThreadStatus>() { new ThreadStatus() { ProblemType = "dvrp" } },
            };
            byte[] rawMsg = _marshaller.Marshall(new Message[] { msg });

            Message outputMessage = null;
            AsyncTcpServer.ResponseCallback c = (byte[] r) =>
            {
                outputMessage = _marshaller.Unmarshall(r)[0];
                waitHandle.Set();
            };
            //_processor.EnqeueInputMessage(rawMsg, c);

            waitHandle.WaitOne(5000);

            Assert.IsInstanceOfType(outputMessage, typeof(ErrorMessage));
        }
    }
}
