using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Messaging;

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
                byte[] buffer = new byte[reader.Length - 3];
                reader.Read(buffer, 0, 3);
                reader.Read(buffer, 0, (int) reader.Length - 3);
                Assert.IsTrue(Marshaller.Unmarshall(buffer).Length == 1);
            }
        }
    }
}
