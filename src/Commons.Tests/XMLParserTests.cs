using System;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class XMLParserTests
    {
        [XmlRoot("Root")]
        public class testingClass
        {
            public string Child1 = "";
            public string Child2 = "";
        }
        [TestMethod]
        public void TestDeserialise()
        {
            XDocument doc1 = new XDocument(
                new XElement("Root",
                    new XElement("Child1", "content1"),
                    new XElement("Child2", "content2")
                )
            );
            var man = new Messaging.XMLParser<testingClass>();
            testingClass tstClass = man.Deserialise(System.Text.Encoding.UTF8.GetBytes(doc1.ToString()));
            Assert.AreEqual("content1", tstClass.Child1);
            Assert.AreEqual("content2", tstClass.Child2);
        }

        [TestMethod]
        public void TestSerialise()
        {
            XDocument doc1 = new XDocument(
                new XElement("Root",
                    new XElement("Child1", "content1"),
                    new XElement("Child2", "content2")
                )
            );
            var tmp = doc1.ToString();
            byte[] buffer;
            var man = new Messaging.XMLParser<testingClass>();
            testingClass tstClass = new testingClass() { Child1 = "content1", Child2 = "content2" };
            man.Serialise(tstClass, out buffer);
            var str = System.Text.Encoding.UTF8.GetString(buffer);
            Assert.AreEqual(tmp.Contains("<Child1>"), str.Contains("<Child1>"));
            Assert.AreEqual(tmp.Contains("<Child2>"), str.Contains("<Child2>"));
        }
    }
}
