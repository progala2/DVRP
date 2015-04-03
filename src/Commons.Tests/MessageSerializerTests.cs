using System.Text;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class MessageSerializerTests
    {
        [TestMethod]
        public void TestDeserialise()
        {
            var serializer = new MessageSerializer();

            var ns = XNamespace.Get("http://www.mini.pw.edu.pl/ucc/");
            var doc = new XDocument(new XDeclaration("1.0", "utf-8", null));
            var root = new XElement(ns + "SolutionRequest", new XElement(ns + "Id", "2"));
            doc.Add(root);

            var str = doc.ToString();
            var tstClass = serializer.Deserialize(Encoding.UTF8.GetBytes(str));
            var solutionRequestMessage = tstClass as SolutionRequestMessage;
            Assert.IsTrue(solutionRequestMessage != null); 
            Assert.IsTrue(2 == solutionRequestMessage.ProblemInstanceId);
        }

        [TestMethod]
        public void TestSerialise()
        {
            var serializer = new MessageSerializer();

             var doc1 = new XDocument(
                 new XElement("SolutionRequest",
                     new XElement("Id", "2")
                 )
             );
             var tmp = doc1.ToString();
             byte[] buffer;
             var tstClass = new SolutionRequestMessage { ProblemInstanceId = 2 };
             buffer = serializer.Serialize(tstClass);
             var str = Encoding.UTF8.GetString(buffer);
             Assert.AreEqual(tmp.Contains("<Id>"), str.Contains("<Id>"));
        }
    }
}
