using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Text;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.Commons.Messaging.Marshalling;
using _15pl04.Ucc.Commons.Messaging.Marshalling.Base;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class SerializationTests
    {
        private ISerializer<Message> _serializer;

        public SerializationTests()
        {
            _serializer = new MessageSerializer();
        }

        [TestMethod]
        public void DivideProblemMessageXmlIsProperlySerializedAndDeserialized()
        {
            var message = new DivideProblemMessage()
            {
                ComputationalNodes = 5,
                TaskManagerId = 10,
                ProblemData = new byte[] { 1, 2, 3, 4, 5 },
                ProblemInstanceId = 15,
                ProblemType = "Dvrp",
            };

            byte[] serializedMessage = _serializer.Serialize(message);
            Message deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsInstanceOfType(deserializedMessage, typeof(DivideProblemMessage));
        }

        [TestMethod]
        public void ErrorMessageXmlIsProperlySerializedAndDeserialized()
        {
            var message = new ErrorMessage()
            {
                ErrorText = "error text example",
                ErrorType = ErrorType.ExceptionOccured,
            };

            byte[] serializedMessage = _serializer.Serialize(message);
            Message deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsInstanceOfType(deserializedMessage, typeof(ErrorMessage));
        }

        [TestMethod]
        public void NoOperationMessageXmlIsProperlySerializedAndDeserialized()
        {
            var message = new NoOperationMessage()
            {
                BackupServers = new List<ServerInfo>(),
            };

            byte[] serializedMessage = _serializer.Serialize(message);
            Message deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsInstanceOfType(deserializedMessage, typeof(NoOperationMessage));
        }

        [TestMethod]
        public void PartialProblemsMessageXmlIsProperlySerializedAndDeserialized()
        {
            var pp = new PartialProblemsMessage.PartialProblem()
            {
                Data = new byte[] { 1, 2, 3 },
                TaskManagerId = 5,
                PartialProblemId = 10,
            };

            var message = new PartialProblemsMessage()
            {
                CommonData = new byte[] { 1, 2, 3, 4, 5 },
                PartialProblems = new List<PartialProblemsMessage.PartialProblem>() { pp },
                ProblemInstanceId = 15,
                ProblemType = "Dvrp",
                SolvingTimeout = 20,
            };

            byte[] serializedMessage = _serializer.Serialize(message);
            Message deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsInstanceOfType(deserializedMessage, typeof(PartialProblemsMessage));
        }

        [TestMethod]
        public void RegisterMessageXmlIsProperlySerializedAndDeserialized()
        {
            var message = new RegisterMessage()
            {
                ComponentType = ComponentType.ComputationalNode,
                Deregistration = true,
                IdToDeregister = 5,
                ParallelThreads = 10,
                SolvableProblems = new List<string>() { "Dvrp" },
            };

            byte[] serializedMessage = _serializer.Serialize(message);
            Message deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsInstanceOfType(deserializedMessage, typeof(RegisterMessage));
        }

        [TestMethod]
        public void RegisterResponseMessageXmlIsProperlySerializedAndDeserialized()
        {
            var si = new ServerInfo()
            {
                IpAddress = "192.168.1.0",
                Port = 9001,
            };

            var message = new RegisterResponseMessage()
            {
                AssignedId = 5,
                BackupServers = new List<ServerInfo>() { si },
                CommunicationTimeout = 50,
            };

            byte[] serializedMessage = _serializer.Serialize(message);
            Message deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsInstanceOfType(deserializedMessage, typeof(RegisterResponseMessage));
        }

        [TestMethod]
        public void SolutionRequestMessageXmlIsProperlySerializedAndDeserialized()
        {
            var message = new SolutionRequestMessage()
            {
                ProblemInstanceId = 5,
            };

            byte[] serializedMessage = _serializer.Serialize(message);
            Message deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsInstanceOfType(deserializedMessage, typeof(SolutionRequestMessage));
        }

        [TestMethod]
        public void SolutionsMessageXmlIsProperlySerializedAndDeserialized()
        {
            var s = new SolutionsMessage.Solution()
            {
                ComputationsTime = 5,
                Data = new byte[] { 1, 2, 3 },
                PartialProblemId = 10,
                TimeoutOccured = true,
                Type = SolutionsMessage.SolutionType.Final
            };

            var message = new SolutionsMessage()
            {
                CommonData = new byte[] { 1, 2, 3 },
                ProblemInstanceId = 5,
                ProblemType = "Dvrp",
                Solutions = new List<SolutionsMessage.Solution>() { s },
            };

            byte[] serializedMessage = _serializer.Serialize(message);
            Message deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsInstanceOfType(deserializedMessage, typeof(SolutionsMessage));
        }

        [TestMethod]
        public void SolveRequestMessageXmlIsProperlySerializedAndDeserialized()
        {
            var message = new SolveRequestMessage()
            {
                ProblemData = new byte[] { 1,2,3},
                ProblemInstanceId = 5,
                ProblemType = "Dvrp",
                SolvingTimeout = 50,
            };

            byte[] serializedMessage = _serializer.Serialize(message);
            Message deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsInstanceOfType(deserializedMessage, typeof(SolveRequestMessage));
        }

        [TestMethod]
        public void SolveRequestResponseMessageXmlIsProperlySerializedAndDeserialized()
        {
            var message = new SolveRequestResponseMessage()
            {
                AssignedId = 5,
            };

            byte[] serializedMessage = _serializer.Serialize(message);
            Message deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsInstanceOfType(deserializedMessage, typeof(SolveRequestResponseMessage));
        }

        [TestMethod]
        public void StatusMessageXmlIsProperlySerializedAndDeserialized()
        {
            var ts = new ThreadStatus()
            {
                PartialProblemId = 5,
                ProblemInstanceId = 10,
                ProblemType = "Dvrp",
                State = ThreadStatus.ThreadState.Busy,
                TimeInThisState = 50,
            };

            var message = new StatusMessage()
            {
                ComponentId = 5,
                Threads = new List<ThreadStatus>() { ts},
            };

            byte[] serializedMessage = _serializer.Serialize(message);
            Message deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsInstanceOfType(deserializedMessage, typeof(StatusMessage));
        }

    }
}
