using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class MessageSerializerTests
    {
        [TestMethod]
        public void TestDeserialise()
        {
            var doc1 = new XDocument(
                new XElement("SolutionRequest",
                    new XElement("Id", "2")
                )
            );
            var tstClass = MessageSerializer.Deserialize(Encoding.UTF8.GetBytes(doc1.ToString()), Message.MessageClassType.SolutionRequest);
            var solutionRequestMessage = tstClass as SolutionRequestMessage;
            Assert.IsTrue(solutionRequestMessage != null); 
            Assert.AreEqual(2, solutionRequestMessage.Id);
        }

        [TestMethod]
        public void TestSerialise()
        {
             var doc1 = new XDocument(
                 new XElement("SolutionRequest",
                     new XElement("Id", "2")
                 )
             );
             var tmp = doc1.ToString();
             byte[] buffer;
             var tstClass = new SolutionRequestMessage { Id = 2 };
             MessageSerializer.Serialize(tstClass, Message.MessageClassType.SolutionRequest, out buffer);
             var str = Encoding.UTF8.GetString(buffer);
             Assert.AreEqual(tmp.Contains("<Id>"), str.Contains("<Id>"));
        }
    }
}
