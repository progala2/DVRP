using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Base;
using _15pl04.Ucc.Commons.Messaging.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class DeserializationTests
    {
        private ISerializer<Message> _serializer;

        public DeserializationTests()
        {
            _serializer = new MessageSerializer();
        }

        [TestMethod]
        public void DivideProblemMessageXmlIsProperlyDeserialized()
        {
            byte[] input = Encoding.UTF8.GetBytes(XmlMessages.DivideProblem);

            Message output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof(DivideProblemMessage));
            Assert.IsNotInstanceOfType(output, typeof(StatusMessage));
        }

        [TestMethod]
        public void ErrorMessageXmlIsProperlyDeserialized()
        {
            byte[] input = Encoding.UTF8.GetBytes(XmlMessages.Error);

            Message output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof(ErrorMessage));
            Assert.IsNotInstanceOfType(output, typeof(StatusMessage));
        }

        [TestMethod]
        public void NoOperationMessageXmlIsProperlyDeserialized()
        {
            byte[] input = Encoding.UTF8.GetBytes(XmlMessages.NoOperation);

            Message output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof(NoOperationMessage));
            Assert.IsNotInstanceOfType(output, typeof(StatusMessage));
        }

        [TestMethod]
        public void PartialProblemsMessageXmlIsProperlyDeserialized()
        {
            byte[] input = Encoding.UTF8.GetBytes(XmlMessages.PartialProblems);

            Message output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof(PartialProblemsMessage));
            Assert.IsNotInstanceOfType(output, typeof(StatusMessage));
        }

        [TestMethod]
        public void RegisterMessageXmlIsProperlyDeserialized()
        {
            byte[] input = Encoding.UTF8.GetBytes(XmlMessages.Register);

            Message output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof(RegisterMessage));
            Assert.IsNotInstanceOfType(output, typeof(StatusMessage));
        }

        [TestMethod]
        public void RegisterResponseMessageXmlIsProperlyDeserialized()
        {
            byte[] input = Encoding.UTF8.GetBytes(XmlMessages.RegisterResponse);

            Message output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof(RegisterResponseMessage));
            Assert.IsNotInstanceOfType(output, typeof(StatusMessage));
        }

        [TestMethod]
        public void SolutionRequestMessageXmlIsProperlyDeserialized()
        {
            byte[] input = Encoding.UTF8.GetBytes(XmlMessages.SolutionRequest);

            Message output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof(SolutionRequestMessage));
            Assert.IsNotInstanceOfType(output, typeof(StatusMessage));
        }

        [TestMethod]
        public void SolutionsMessageXmlIsProperlyDeserialized()
        {
            byte[] input = Encoding.UTF8.GetBytes(XmlMessages.Solutions);

            Message output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof(SolutionsMessage));
            Assert.IsNotInstanceOfType(output, typeof(StatusMessage));
        }

        [TestMethod]
        public void SolveRequestMessageXmlIsProperlyDeserialized()
        {
            byte[] input = Encoding.UTF8.GetBytes(XmlMessages.SolveRequest);

            Message output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof(SolveRequestMessage));
            Assert.IsNotInstanceOfType(output, typeof(StatusMessage));
        }

        [TestMethod]
        public void SolveRequestResponseMessageXmlIsProperlyDeserialized()
        {
            byte[] input = Encoding.UTF8.GetBytes(XmlMessages.SolveRequestResponse);

            Message output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof(SolveRequestResponseMessage));
            Assert.IsNotInstanceOfType(output, typeof(StatusMessage));
        }

        [TestMethod]
        public void StatusMessageXmlIsProperlyDeserialized()
        {
            byte[] input = Encoding.UTF8.GetBytes(XmlMessages.Status);

            Message output = _serializer.Deserialize(input);

            Assert.IsInstanceOfType(output, typeof(StatusMessage));
            Assert.IsNotInstanceOfType(output, typeof(RegisterMessage));
        }
    }
}
