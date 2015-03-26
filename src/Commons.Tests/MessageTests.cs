using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class MessageTests
    {
        [TestMethod]
        public void GettingXsdFileContentReturnsNonEmptyStringForDerivedTypes()
        {
            var typeOfMessage = typeof(Message);
            var assembly = Assembly.GetAssembly(typeOfMessage);
            var types = assembly.GetTypes();
            foreach (var xsdFileContent in from type in types where typeOfMessage.IsAssignableFrom(type) && !type.IsAbstract select Message.GetXsdFileContent(type))
            {
                Assert.IsFalse(string.IsNullOrEmpty(xsdFileContent));
            }
        }
    }
}
