using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Tests.Tools;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class MessagesTests
    {
        [TestMethod]
        public void NoOperationMessageTest()
        {
            var msg = XmlFileParser<NoOperationMessage>.Deserialize("XmlMessages/NoOperation.xml");
            Assert.IsTrue(msg.BackupCommunicationServers.Count == 1);
        }

        [TestMethod]
        public void PartialProblemsMessageTest()
        {
            var msg = XmlFileParser<PartialProblemsMessage>.Deserialize("XmlMessages/PartialProblems.xml");
            Assert.IsTrue(msg.Id == 12);
        }

        [TestMethod]
        public void RegisterMessageTest()
        {
            var msg = XmlFileParser<RegisterMessage>.Deserialize("XmlMessages/Register.xml");
            Assert.IsTrue(msg.Id == 12);
            Assert.IsTrue(msg.Deregister == null);
        }

        [TestMethod]
        public void RegisterResponseMessageTest()
        {
            var msg = XmlFileParser<RegisterResponseMessage>.Deserialize("XmlMessages/RegisterResponse.xml");
            Assert.IsTrue(msg.Id == 12);
        }

        [TestMethod]
        public void SolutionsMessageTest()
        {
            var msg = XmlFileParser<SolutionsMessage>.Deserialize("XmlMessages/Solution.xml");
            Assert.IsTrue(msg.Id == 12);
        }

        [TestMethod]
        public void SolutionRequestMessageTest()
        {
            var msg = XmlFileParser<SolutionRequestMessage>.Deserialize("XmlMessages/SolutionRequest.xml");
            Assert.IsTrue(msg.Id == 12);
        }

        [TestMethod]
        public void SolveRequestMessageTest()
        {
            var msg = XmlFileParser<SolveRequestMessage>.Deserialize("XmlMessages/SolveRequest.xml");
            Assert.IsTrue(msg.Id == 12);
        }

        [TestMethod]
        public void SolveRequestResponseMessageTest()
        {
            var msg = XmlFileParser<SolveRequestResponseMessage>.Deserialize("XmlMessages/SolveRequestResponse.xml");
            Assert.IsTrue(msg.Id == 12);
        }

        [TestMethod]
        public void StatusMessageTest()
        {
            var msg = XmlFileParser<StatusMessage>.Deserialize("XmlMessages/Status.xml");
            Assert.IsTrue(msg.Id == 12);
        }
    }
}
