using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class MarshallerTests
    {
        [TestMethod]
        public void TestUnmarshall()
        {
            using (var reader = new FileStream("XmlMessages/NoOperation.xml", FileMode.Open))
            {
                var buffer = new byte[(reader.Length - 2)*2];
                reader.Position = 3;
                reader.Read(buffer, 0, (int) reader.Length - 3);
                reader.Position = 3;
                buffer[buffer.Length - 1] = buffer[reader.Length - 3] = 23;
                reader.Read(buffer, (int)reader.Length - 2, (int)reader.Length - 3);
                Assert.IsTrue((new Marshaller()).Unmarshall(buffer).Length == 2);
            }
        }

        [TestMethod]
        public void TestMarshall()
        {
            Message[] tstClass = { new SolutionRequestMessage { Id = 2 }, new SolutionRequestMessage { Id = 3 } };
            Assert.IsTrue((new Marshaller()).Marshall(tstClass).Count(i => i == 23) == 2);
        }
    }
}
