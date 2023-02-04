using System.Collections.Generic;
using Dvrp.Ucc.Commons.Components;
using Dvrp.Ucc.Commons.Messaging.Marshalling;
using Dvrp.Ucc.Commons.Messaging.Models;
using Xunit;

namespace Dvrp.Ucc.Commons.Tests
{
	public class JsonSerializationTests
	{
        
		private readonly Marshaller _serializer;
		public JsonSerializationTests()
		{
			_serializer = new Marshaller();
		}

		[Fact]
		public void DivideProblemMessageXmlIsProperlyDeserialized()
		{
			var msg = new DivideProblemMessage("problemType", 1, new byte[] { 1, 2, 3 }, 1, 1);
            var obj = _serializer.Serialize(msg);

			var output = _serializer.Deserialize(obj);

			Assert.IsType<DivideProblemMessage>(output[0]);
			Assert.IsNotType<StatusMessage>(output[0]);
		}

        [Fact]
        public void ErrorMessageXmlIsProperlyDeserialized()
        {
            var message = new ErrorMessage
            {
                ErrorText = "error text example",
                ErrorType = ErrorType.ExceptionOccured
            };
            var input = _serializer.Serialize(message);

            var output = _serializer.Deserialize(input);

            Assert.IsType<ErrorMessage>(output[0]);
            Assert.IsNotType<StatusMessage>(output[0]);
        }

		[Fact]
		public void NoOperationMessageXmlIsProperlyDeserialized()
		{
			var message = new NoOperationMessage
			{
				BackupServers = new List<ServerInfo>()
			};
			var input = _serializer.Serialize(message);

			var output = _serializer.Deserialize(input);

			Assert.IsType<NoOperationMessage>(output[0]);
			Assert.IsNotType<StatusMessage>(output[0]);
		}

		[Fact]
		public void PartialProblemsMessageXmlIsProperlyDeserialized()
		{
			var pp = new PartialProblemsMessage.PartialProblem(5, new byte[] { 1, 2, 3 }, 10);

			var message = new PartialProblemsMessage("Dvrp")
			{
				CommonData = new byte[] { 1, 2, 3, 4, 5 },
				PartialProblems = new List<PartialProblemsMessage.PartialProblem> { pp },
				ProblemInstanceId = 15,
				SolvingTimeout = 20
			};
			var input = _serializer.Serialize(message);

			var output = _serializer.Deserialize(input);

			Assert.IsType<PartialProblemsMessage>(output[0]);
			Assert.IsNotType<StatusMessage>(output[0]);
		}

		[Fact]
		public void RegisterMessageXmlIsProperlyDeserialized()
		{
			var message = new RegisterMessage
			{
				ComponentType = ComponentType.ComputationalNode,
				Deregistration = true,
				Id = 5,
				ParallelThreads = 10,
				SolvableProblems = new List<string> { "Dvrp" }
			};
			var input = _serializer.Serialize(message);

			var output = _serializer.Deserialize(input);

			Assert.IsType<RegisterMessage>(output[0]);
			Assert.IsNotType<StatusMessage>(output[0]);
		}

		[Fact]
		public void RegisterResponseMessageXmlIsProperlyDeserialized()
		{
			var si = new ServerInfo("192.168.1.0", 9001);

			var message = new RegisterResponseMessage
			{
				AssignedId = 5,
				BackupServers = new List<ServerInfo> { si },
				CommunicationTimeout = 50
			};
			var input = _serializer.Serialize(message);

			var output = _serializer.Deserialize(input);

			Assert.IsType<RegisterResponseMessage>(output[0]);
			Assert.IsNotType<StatusMessage>(output[0]);
		}

		[Fact]
		public void SolutionRequestMessageXmlIsProperlyDeserialized()
		{
			var message = new SolutionRequestMessage
			{
				ProblemInstanceId = 5
			};
			var input = _serializer.Serialize(message);

			var output = _serializer.Deserialize(input);

			Assert.IsType<SolutionRequestMessage>(output[0]);
			Assert.IsNotType<StatusMessage>(output[0]);
		}

		[Fact]
		public void SolutionsMessageXmlIsProperlyDeserialized()
		{
			var s = new SolutionsMessage.Solution
			{
				ComputationsTime = 5,
				Data = new byte[] { 1, 2, 3 },
				PartialProblemId = 10,
				TimeoutOccured = true,
				Type = SolutionsMessage.SolutionType.Final
			};

			var message = new SolutionsMessage("Dvrp")
			{
				CommonData = new byte[] { 1, 2, 3 },
				ProblemInstanceId = 5,
				Solutions = new List<SolutionsMessage.Solution> { s }
			};
			var input = _serializer.Serialize(message);

			var output = _serializer.Deserialize(input);

			Assert.IsType<SolutionsMessage>(output[0]);
			Assert.IsNotType<StatusMessage>(output[0]);
		}

		[Fact]
		public void SolveRequestMessageXmlIsProperlyDeserialized()
		{
			var message = new SolveRequestMessage("Dvrp", new byte[] { 1, 2, 3 }, 1, 1);
			var input = _serializer.Serialize(message);

			var output = _serializer.Deserialize(input);

			Assert.IsType<SolveRequestMessage>(output[0]);
			Assert.IsNotType<StatusMessage>(output[0]);
		}

		[Fact]
		public void SolveRequestResponseMessageXmlIsProperlyDeserialized()
		{
			var message = new SolveRequestResponseMessage
			{
				AssignedId = 5
			};
			var input = _serializer.Serialize(message);

			var output = _serializer.Deserialize(input);

			Assert.IsType<SolveRequestResponseMessage>(output[0]);
			Assert.IsNotType<StatusMessage>(output[0]);
		}

		[Fact]
		public void StatusMessageXmlIsProperlyDeserialized()
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
				Threads = new List<ThreadStatus> { ts }
			};
			var input = _serializer.Serialize(message);

			var output = _serializer.Deserialize(input);

			Assert.IsType<StatusMessage>(output[0]);
			Assert.IsNotType<RegisterMessage>(output[0]);
		}
	}
}