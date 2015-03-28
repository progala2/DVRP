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
            var ns = XNamespace.Get("http://www.mini.pw.edu.pl/ucc/");
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
            var root = new XElement(ns + "SolutionRequest", new XElement(ns + "Id", "2"));
            doc.Add(root);

            var str = doc.ToString();
            var tstClass = MessageSerializer.Deserialize(Encoding.UTF8.GetBytes(str), Message.MessageClassType.SolutionRequest);
            var solutionRequestMessage = tstClass as SolutionRequestMessage;
            Assert.IsTrue(solutionRequestMessage != null); 
            Assert.IsTrue(2 == solutionRequestMessage.Id);
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
