using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class MessageValidatorTests
    {
        [TestMethod]
        public void TestValidate()
        {
            using (var reader = new FileStream("XmlMessages/NoOperation.xml", FileMode.Open))
            {
                var buffer = new byte[reader.Length - 3];
                reader.Read(buffer, 0, 3);
                reader.Read(buffer, 0, (int)reader.Length - 3);
                var str = Encoding.ASCII.GetString(buffer);
                MessageValidator.Validate(str, Message.MessageClassType.NoOperation);
                try
                {
                    MessageValidator.Validate(str, Message.MessageClassType.Status);
                }
                catch (Exception)
                {
                    return;
                }
                Assert.Fail();
            }
            
        }
    }
}
