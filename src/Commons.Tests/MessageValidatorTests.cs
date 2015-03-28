using System;
using System.IO;
using System.Text;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class MessageValidatorTests
    {
        [TestMethod]
        public void DivideProblemMessageValidation()
        {
            var validationResult = Validate("XmlMessages/DivideProblem.xml", Message.MessageClassType.DivideProblem);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void ErrorMessageValidation()
        {
            var validationResult = Validate("XmlMessages/Error.xml", Message.MessageClassType.Error);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void NoOperationMessageValidation()
        {
            var validationResult = Validate("XmlMessages/NoOperation.xml", Message.MessageClassType.NoOperation);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void PartialProblemsMessageValidation()
        {
            var validationResult = Validate("XmlMessages/PartialProblems.xml", Message.MessageClassType.PartialProblems);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void RegisterMessageValidation()
        {
            var validationResult = Validate("XmlMessages/Register.xml", Message.MessageClassType.Register);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void RegisterResponseMessageValidation()
        {
            var validationResult = Validate("XmlMessages/RegisterResponse.xml", Message.MessageClassType.RegisterResponse);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void SolutionRequestMessageValidation()
        {
            var validationResult = Validate("XmlMessages/SolutionRequest.xml", Message.MessageClassType.SolutionRequest);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void SolutionsMessageValidation()
        {
            var validationResult = Validate("XmlMessages/Solutions.xml", Message.MessageClassType.Solutions);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void SolveRequestMessageValidation()
        {
            var validationResult = Validate("XmlMessages/SolveRequest.xml", Message.MessageClassType.SolveRequest);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void SolveRequestResponseMessageValidation()
        {
            var validationResult = Validate("XmlMessages/SolveRequestResponse.xml", Message.MessageClassType.SolveRequestResponse);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void StatusMessageValidation()
        {
            var validationResult = Validate("XmlMessages/Status.xml", Message.MessageClassType.Status);
            Assert.IsTrue(validationResult);
        }

        private bool Validate(string xmlFilePath, Message.MessageClassType typeOfMessageToValidateWith)
        {
            using (var reader = new StreamReader(xmlFilePath, Encoding.UTF8))
            {
                var fileContent = reader.ReadToEnd();
                try
                {
                    MessageValidator.Validate(fileContent, typeOfMessageToValidateWith);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
