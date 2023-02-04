using System.Text;
using Dvrp.Ucc.Commons.Messaging.Marshalling;
using Dvrp.Ucc.Commons.Messaging.Marshalling.Base;
using Dvrp.Ucc.Commons.Messaging.Models;
using Dvrp.Ucc.Commons.Messaging.Models.Base;
using Xunit;

namespace Dvrp.Ucc.Commons.Tests
{
    public class DeserializationTests
    {
        private readonly ISerializer<Message> _serializer;

        public DeserializationTests()
        {
            _serializer = new MessageSerializer();
        }

        [Fact]
        public void DivideProblemMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.DivideProblem);

            var output = _serializer.Deserialize(input);

            Assert.IsType<DivideProblemMessage>(output);
            Assert.IsNotType<StatusMessage>(output);
        }

        [Fact]
        public void ErrorMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.Error);

            var output = _serializer.Deserialize(input);

            Assert.IsType<ErrorMessage>(output);
            Assert.IsNotType<StatusMessage>(output);
        }

        [Fact]
        public void NoOperationMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.NoOperation);

            var output = _serializer.Deserialize(input);

            Assert.IsType<NoOperationMessage>(output);
            Assert.IsNotType<StatusMessage>(output);
        }

        [Fact]
        public void PartialProblemsMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.PartialProblems);

            var output = _serializer.Deserialize(input);

            Assert.IsType<PartialProblemsMessage>(output);
            Assert.IsNotType<StatusMessage>(output);
        }

        [Fact]
        public void RegisterMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.Register);

            var output = _serializer.Deserialize(input);

            Assert.IsType<RegisterMessage>(output);
            Assert.IsNotType<StatusMessage>(output);
        }

        [Fact]
        public void RegisterResponseMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.RegisterResponse);

            var output = _serializer.Deserialize(input);

            Assert.IsType<RegisterResponseMessage>(output);
            Assert.IsNotType<StatusMessage>(output);
        }

        [Fact]
        public void SolutionRequestMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.SolutionRequest);

            var output = _serializer.Deserialize(input);

            Assert.IsType<SolutionRequestMessage>(output);
            Assert.IsNotType<StatusMessage>(output);
        }

        [Fact]
        public void SolutionsMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.Solutions);

            var output = _serializer.Deserialize(input);

            Assert.IsType<SolutionsMessage>(output);
            Assert.IsNotType<StatusMessage>(output);
        }

        [Fact]
        public void SolveRequestMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.SolveRequest);

            var output = _serializer.Deserialize(input);

            Assert.IsType<SolveRequestMessage>(output);
            Assert.IsNotType<StatusMessage>(output);
        }

        [Fact]
        public void SolveRequestResponseMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.SolveRequestResponse);

            var output = _serializer.Deserialize(input);

            Assert.IsType<SolveRequestResponseMessage>(output);
            Assert.IsNotType<StatusMessage>(output);
        }

        [Fact]
        public void StatusMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.Status);

            var output = _serializer.Deserialize(input);

            Assert.IsType<StatusMessage>(output);
            Assert.IsNotType<RegisterMessage>(output);
        }
    }
}