using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Messaging.Marshalling;
using _15pl04.Ucc.Commons.Messaging.Marshalling.Base;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class DeserializationTests
    {
        private readonly ISerializer<Message> _serializer;

        public DeserializationTests()
        {
            _serializer = new MessageSerializer();
        }

        [TestMethod]
        public void DivideProblemMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.DivideProblem);

            var output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof (DivideProblemMessage));
            Assert.IsNotInstanceOfType(output, typeof (StatusMessage));
        }

        [TestMethod]
        public void ErrorMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.Error);

            var output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof (ErrorMessage));
            Assert.IsNotInstanceOfType(output, typeof (StatusMessage));
        }

        [TestMethod]
        public void NoOperationMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.NoOperation);

            var output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof (NoOperationMessage));
            Assert.IsNotInstanceOfType(output, typeof (StatusMessage));
        }

        [TestMethod]
        public void PartialProblemsMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.PartialProblems);

            var output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof (PartialProblemsMessage));
            Assert.IsNotInstanceOfType(output, typeof (StatusMessage));
        }

        [TestMethod]
        public void RegisterMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.Register);

            var output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof (RegisterMessage));
            Assert.IsNotInstanceOfType(output, typeof (StatusMessage));
        }

        [TestMethod]
        public void RegisterResponseMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.RegisterResponse);

            var output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof (RegisterResponseMessage));
            Assert.IsNotInstanceOfType(output, typeof (StatusMessage));
        }

        [TestMethod]
        public void SolutionRequestMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.SolutionRequest);

            var output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof (SolutionRequestMessage));
            Assert.IsNotInstanceOfType(output, typeof (StatusMessage));
        }

        [TestMethod]
        public void SolutionsMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.Solutions);

            var output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof (SolutionsMessage));
            Assert.IsNotInstanceOfType(output, typeof (StatusMessage));
        }

        [TestMethod]
        public void SolveRequestMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.SolveRequest);

            var output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof (SolveRequestMessage));
            Assert.IsNotInstanceOfType(output, typeof (StatusMessage));
        }

        [TestMethod]
        public void SolveRequestResponseMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.SolveRequestResponse);

            var output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof (SolveRequestResponseMessage));
            Assert.IsNotInstanceOfType(output, typeof (StatusMessage));
        }

        [TestMethod]
        public void StatusMessageXmlIsProperlyDeserialized()
        {
            var input = Encoding.UTF8.GetBytes(XmlMessages.Status);

            var output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof (StatusMessage));
            Assert.IsNotInstanceOfType(output, typeof (RegisterMessage));
        }
    }
}