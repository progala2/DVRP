using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Base;
using _15pl04.Ucc.Commons.Messaging.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class SerializationTests
    {
        private ISerializer<Message> _serializer;

        public SerializationTests()
        {
            _serializer = new MessageSerializer();
        }

        [TestMethod]
        public void DivideProblemMessageXmlIsProperlySerializedAndDeserialized()
        {
            var message = new DivideProblemMessage()
            {
                ComputationalNodes = 5,
                NodeId = 10,
                ProblemData = new byte[] { 1, 2, 3, 4, 5 },
                ProblemInstanceId = 15,
                ProblemType = "Dvrp",
            };

            byte[] serializedMessage = _serializer.Serialize(message);
            Message deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsInstanceOfType(deserializedMessage, typeof(DivideProblemMessage));
        }

        [TestMethod]
        public void ErrorMessageXmlIsProperlySerializedAndDeserialized()
        {
            var message = new ErrorMessage()
            {
                ErrorText = "error text example",
                ErrorType = ErrorType.ExceptionOccured,
            };

            byte[] serializedMessage = _serializer.Serialize(message);
            Message deserializedMessage = _serializer.Deserialize(serializedMessage);

            Assert.IsInstanceOfType(deserializedMessage, typeof(ErrorMessage));
        }

        // TODO rest of messages
    }
}
