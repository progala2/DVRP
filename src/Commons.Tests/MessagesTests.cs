using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Tests.Tools;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class MessagesTests
    {
        [TestMethod]
        public void NoOperationMessageDeserialization()
        {
            var msg = XmlFileParser<NoOperationMessage>.Deserialize("XmlMessages/NoOperation.xml");
            Assert.IsTrue(msg.BackupServers.Count == 1);
        }

        [TestMethod]
        public void PartialProblemsMessageDeserialization()
        {
            var msg = XmlFileParser<PartialProblemsMessage>.Deserialize("XmlMessages/PartialProblems.xml");
            Assert.IsTrue(msg.ProblemInstanceId == 12);
        }

        [TestMethod]
        public void RegisterMessageDeserialization()
        {
            var msg = XmlFileParser<RegisterMessage>.Deserialize("XmlMessages/Register.xml");
            Assert.IsTrue(msg.IdToDeregister == 12);
            Assert.IsTrue(msg.Deregistration == null);
        }

        [TestMethod]
        public void RegisterResponseMessageDeserialization()
        {
            var msg = XmlFileParser<RegisterResponseMessage>.Deserialize("XmlMessages/RegisterResponse.xml");
            Assert.IsTrue(msg.AssignedId == 12);
        }

        [TestMethod]
        public void SolutionsMessageDeserialization()
        {
            var msg = XmlFileParser<SolutionsMessage>.Deserialize("XmlMessages/Solutions.xml");
            Assert.IsTrue(msg.ProblemInstanceId == 12);
        }

        [TestMethod]
        public void SolutionRequestMessageDeserialization()
        {
            var msg = XmlFileParser<SolutionRequestMessage>.Deserialize("XmlMessages/SolutionRequest.xml");
            Assert.IsTrue(msg.ProblemInstanceId == 12);
        }

        [TestMethod]
        public void SolveRequestMessageDeserialization()
        {
            var msg = XmlFileParser<SolveRequestMessage>.Deserialize("XmlMessages/SolveRequest.xml");
            Assert.IsTrue(msg.ProblemInstanceId == 12);
        }

        [TestMethod]
        public void SolveRequestResponseMessageDeserialization()
        {
            var msg = XmlFileParser<SolveRequestResponseMessage>.Deserialize("XmlMessages/SolveRequestResponse.xml");
            Assert.IsTrue(msg.AssignedId == 12);
        }

        [TestMethod]
        public void StatusMessageDeserialization()
        {
            var msg = XmlFileParser<StatusMessage>.Deserialize("XmlMessages/Status.xml");
            Assert.IsTrue(msg.ComponentId == 12);
        }
    }
}
