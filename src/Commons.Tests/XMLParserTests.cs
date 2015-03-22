using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Messaging;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class XmlParserTests
    {
        [XmlRoot("Root")]
        public class TestingClass
        {
            public string Child1 = "";
            public string Child2 = "";
        }
        [TestMethod]
        public void TestDeserialise()
        {
            var doc1 = new XDocument(
                new XElement("Root",
                    new XElement("Child1", "content1"),
                    new XElement("Child2", "content2")
                )
            );
            var tstClass = XmlParser<TestingClass>.Deserialize(Encoding.UTF8.GetBytes(doc1.ToString()));
            Assert.AreEqual("content1", tstClass.Child1);
            Assert.AreEqual("content2", tstClass.Child2);
        }

        [TestMethod]
        public void TestSerialise()
        {
            var doc1 = new XDocument(
                new XElement("Root",
                    new XElement("Child1", "content1"),
                    new XElement("Child2", "content2")
                )
            );
            var tmp = doc1.ToString();
            byte[] buffer;
            var tstClass = new TestingClass() { Child1 = "content1", Child2 = "content2" };
            XmlParser<TestingClass>.Serialize(tstClass, out buffer);
            var str = Encoding.UTF8.GetString(buffer);
            Assert.AreEqual(tmp.Contains("<Child1>"), str.Contains("<Child1>"));
            Assert.AreEqual(tmp.Contains("<Child2>"), str.Contains("<Child2>"));
        }
    }
}
