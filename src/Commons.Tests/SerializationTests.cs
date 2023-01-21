using System.Collections.Generic;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.Commons.Messaging.Marshalling;
using _15pl04.Ucc.Commons.Messaging.Marshalling.Base;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;
using Xunit;

namespace _15pl04.Ucc.Commons.Tests
{
    public class SerializationTests
    {
        private readonly ISerializer<Message> _serializer;

        public SerializationTests()
        {
            _serializer = new MessageSerializer();
        }

        [Fact]
        public void DivideProblemMessageXmlIsProperlySerializedAndDeserialized()
        {
            var message = new DivideProblemMessage
            {
                ComputationalNodes = 5,
                TaskManagerId = 10,
                ProblemData = new byte[] {1, 2, 3, 4, 5},
                ProblemInstanceId = 15,
                ProblemType = "Dvrp"
            };

            var serializedMessage = _serializer.Serialize(message);
            var deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsType<DivideProblemMessage>(deserializedMessage);
        }

        [Fact]
        public void ErrorMessageXmlIsProperlySerializedAndDeserialized()
        {
            var message = new ErrorMessage
            {
                ErrorText = "error text example",
                ErrorType = ErrorType.ExceptionOccured
            };

            var serializedMessage = _serializer.Serialize(message);
            var deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsType<ErrorMessage>(deserializedMessage);
        }

        [Fact]
        public void NoOperationMessageXmlIsProperlySerializedAndDeserialized()
        {
            var message = new NoOperationMessage
            {
                BackupServers = new List<ServerInfo>()
            };

            var serializedMessage = _serializer.Serialize(message);
            var deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsType<NoOperationMessage>(deserializedMessage);
        }

        [Fact]
        public void PartialProblemsMessageXmlIsProperlySerializedAndDeserialized()
        {
            var pp = new PartialProblemsMessage.PartialProblem
            {
                Data = new byte[] {1, 2, 3},
                TaskManagerId = 5,
                PartialProblemId = 10
            };

            var message = new PartialProblemsMessage
            {
                CommonData = new byte[] {1, 2, 3, 4, 5},
                PartialProblems = new List<PartialProblemsMessage.PartialProblem> {pp},
                ProblemInstanceId = 15,
                ProblemType = "Dvrp",
                SolvingTimeout = 20
            };

            var serializedMessage = _serializer.Serialize(message);
            var deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsType<PartialProblemsMessage>(deserializedMessage);
        }

        [Fact]
        public void RegisterMessageXmlIsProperlySerializedAndDeserialized()
        {
            var message = new RegisterMessage
            {
                ComponentType = ComponentType.ComputationalNode,
                Deregistration = true,
                Id = 5,
                ParallelThreads = 10,
                SolvableProblems = new List<string> {"Dvrp"}
            };

            var serializedMessage = _serializer.Serialize(message);
            var deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsType<RegisterMessage>(deserializedMessage);
        }

        [Fact]
        public void RegisterResponseMessageXmlIsProperlySerializedAndDeserialized()
        {
            var si = new ServerInfo
            {
                IpAddress = "192.168.1.0",
                Port = 9001
            };

            var message = new RegisterResponseMessage
            {
                AssignedId = 5,
                BackupServers = new List<ServerInfo> {si},
                CommunicationTimeout = 50
            };

            var serializedMessage = _serializer.Serialize(message);
            var deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsType<RegisterResponseMessage>(deserializedMessage);
        }

        [Fact]
        public void SolutionRequestMessageXmlIsProperlySerializedAndDeserialized()
        {
            var message = new SolutionRequestMessage
            {
                ProblemInstanceId = 5
            };

            var serializedMessage = _serializer.Serialize(message);
            var deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsType<SolutionRequestMessage>(deserializedMessage);
        }

        [Fact]
        public void SolutionsMessageXmlIsProperlySerializedAndDeserialized()
        {
            var s = new SolutionsMessage.Solution
            {
                ComputationsTime = 5,
                Data = new byte[] {1, 2, 3},
                PartialProblemId = 10,
                TimeoutOccured = true,
                Type = SolutionsMessage.SolutionType.Final
            };

            var message = new SolutionsMessage
            {
                CommonData = new byte[] {1, 2, 3},
                ProblemInstanceId = 5,
                ProblemType = "Dvrp",
                Solutions = new List<SolutionsMessage.Solution> {s}
            };

            var serializedMessage = _serializer.Serialize(message);
            var deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsType<SolutionsMessage>(deserializedMessage);
        }

        [Fact]
        public void SolveRequestMessageXmlIsProperlySerializedAndDeserialized()
        {
            var message = new SolveRequestMessage
            {
                ProblemData = new byte[] {1, 2, 3},
                ProblemInstanceId = 5,
                ProblemType = "Dvrp",
                SolvingTimeout = 50
            };

            var serializedMessage = _serializer.Serialize(message);
            var deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsType<SolveRequestMessage>(deserializedMessage);
        }

        [Fact]
        public void SolveRequestResponseMessageXmlIsProperlySerializedAndDeserialized()
        {
            var message = new SolveRequestResponseMessage
            {
                AssignedId = 5
            };

            var serializedMessage = _serializer.Serialize(message);
            var deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsType<SolveRequestResponseMessage>(deserializedMessage);
        }

        [Fact]
        public void StatusMessageXmlIsProperlySerializedAndDeserialized()
        {
            var ts = new ThreadStatus
            {
                PartialProblemId = 5,
                ProblemInstanceId = 10,
                ProblemType = "Dvrp",
                State = ThreadStatus.ThreadState.Busy,
                TimeInThisState = 50
            };

            var message = new StatusMessage
            {
                ComponentId = 5,
                Threads = new List<ThreadStatus> {ts}
            };

            var serializedMessage = _serializer.Serialize(message);
            var deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsType<StatusMessage>(deserializedMessage);
        }
    }
}