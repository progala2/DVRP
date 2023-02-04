using System;
using System.Collections.Generic;
using System.Threading;
using Dvrp.Ucc.Commons.Components;
using Dvrp.Ucc.Commons.Messaging.Marshalling;
using Dvrp.Ucc.Commons.Messaging.Marshalling.Base;
using Dvrp.Ucc.Commons.Messaging.Models;
using Dvrp.Ucc.Commons.Messaging.Models.Base;
using Dvrp.Ucc.CommunicationServer.Components;
using Dvrp.Ucc.CommunicationServer.Components.Base;
using Dvrp.Ucc.CommunicationServer.Messaging;
using Dvrp.Ucc.CommunicationServer.Messaging.Base;
using Dvrp.Ucc.CommunicationServer.WorkManagement;
using Dvrp.Ucc.CommunicationServer.WorkManagement.Base;
using Xunit;
using ErrorMessage = Dvrp.Ucc.Commons.Messaging.Models.ErrorMessage;

namespace Dvrp.Ucc.CommunicationServer.Tests
{
	public class MessageProcessorTests
	{
		private readonly IMarshaller<Message> _marshaller;
		private readonly MessageProcessor _processor;

		public MessageProcessorTests()
		{
			_marshaller = new Marshaller();

			IComponentOverseer overseer = new ComponentOverseer(5, 1);
			IWorkManager workManager = new WorkManager(overseer);

			_processor = new MessageProcessor(overseer, workManager);
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
				SolvableProblems = new List<string> { "dvrp" }
			};

			var rawMsg = _marshaller.Serialize(new Message[] { msg });

			ProcessedDataCallback c = _ =>
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
				SolvableProblems = new List<string> { "dvrp" },
				ComponentType = ComponentType.ComputationalNode
			};
			var rawMsg = _marshaller.Serialize(new Message[] { msg });

			Message? outputMessage = null;
			ProcessedDataCallback c = response =>
			{
				outputMessage = _marshaller.Deserialize(response)[0];
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

			var msg = new SolveRequestMessage("dvrp", Array.Empty<byte>());
			var rawMsg = _marshaller.Serialize(msg);

			Message? outputMessage = null;
			ProcessedDataCallback c = response =>
			{
				outputMessage = _marshaller.Deserialize(response)[0];
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
				Threads = new List<ThreadStatus> { new ThreadStatus { ProblemType = "dvrp" } }
			};
			var rawMsg = _marshaller.Serialize(new Message[] { msg });

			Message? outputMessage = null;
			ProcessedDataCallback c = response =>
			{
				outputMessage = _marshaller.Deserialize(response)[0];
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
				Threads = new List<ThreadStatus> { new ThreadStatus { ProblemType = "dvrp" } }
			};
			var rawMsg = _marshaller.Serialize(new Message[] { msg });

			Message? outputMessage = null;
			ProcessedDataCallback c = response =>
			{
				outputMessage = _marshaller.Deserialize(response)[0];
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