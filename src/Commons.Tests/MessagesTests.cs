using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml.Serialization;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class MessagesTests
    {
        static class XmlParser<T>
        {
            private static readonly Type Type = typeof(T);

            public static T Deserialize(string path)
            {
                using (var reader = new FileStream(path, FileMode.Open))
                {
                    var xml = new XmlSerializer(Type);
                    var instance = (T)xml.Deserialize(reader);
                    return instance;
                }
            }
        }

        [TestMethod]
        public void NoOperationMessageTest()
        {
            var msg = XmlParser<NoOperationMessage>.Deserialize("XmlMessages/NoOperation.xml");
            Assert.IsTrue(msg.BackupCommunicationServers.Count == 1);
        }

        [TestMethod]
        public void PartialProblemsMessageTest()
        {
            var msg = XmlParser<PartialProblemsMessage>.Deserialize("XmlMessages/PartialProblems.xml");
            Assert.IsTrue(msg.Id == 12);
        }

        [TestMethod]
        public void RegisterMessageTest()
        {
            var msg = XmlParser<RegisterMessage>.Deserialize("XmlMessages/Register.xml");
            Assert.IsTrue(msg.Id == 12);
        }

        [TestMethod]
        public void RegisterResponseMessageTest()
        {
            var msg = XmlParser<RegisterResponseMessage>.Deserialize("XmlMessages/RegisterResponse.xml");
            Assert.IsTrue(msg.Id == 12);
        }

        [TestMethod]
        public void SolutionsMessageTest()
        {
            var msg = XmlParser<SolutionsMessage>.Deserialize("XmlMessages/Solution.xml");
            Assert.IsTrue(msg.Id == 12);
        }

        [TestMethod]
        public void SolutionRequestMessageTest()
        {
            var msg = XmlParser<SolutionRequestMessage>.Deserialize("XmlMessages/SolutionRequest.xml");
            Assert.IsTrue(msg.Id == 12);
        }

        [TestMethod]
        public void SolveRequestMessageTest()
        {
            var msg = XmlParser<SolveRequestMessage>.Deserialize("XmlMessages/SolveRequest.xml");
            Assert.IsTrue(msg.Id == 12);
        }

        [TestMethod]
        public void SolveRequestResponseMessageTest()
        {
            var msg = XmlParser<SolveRequestResponseMessage>.Deserialize("XmlMessages/SolveRequestResponse.xml");
            Assert.IsTrue(msg.Id == 12);
        }

        [TestMethod]
        public void StatusMessageTest()
        {
            var msg = XmlParser<StatusMessage>.Deserialize("XmlMessages/Status.xml");
            Assert.IsTrue(msg.Id == 12);
        }
    }
}
