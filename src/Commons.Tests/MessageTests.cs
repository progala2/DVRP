using System;
using System.Reflection;
using _15pl04.Ucc.Commons.Messaging.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            foreach (var type in types)
            {
                if (typeOfMessage.IsAssignableFrom(type) && !type.IsAbstract)
                {
                    var instance = (Message)Activator.CreateInstance(type);
                    var xsdFileContent = instance.GetXsdFileContent();
                    Assert.IsFalse(string.IsNullOrEmpty(xsdFileContent));
                }
            }
        }
    }
}
