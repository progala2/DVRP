using System;
using System.IO;
using System.Text;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class MessageValidatorTests
    {
        [TestMethod]
        public void DivideProblemMessageValidation()
        {
            var validationResult = Validate("XmlMessages/DivideProblem.xml", MessageClass.DivideProblem);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void ErrorMessageValidation()
        {
            var validationResult = Validate("XmlMessages/Error.xml", MessageClass.Error);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void NoOperationMessageValidation()
        {
            var validationResult = Validate("XmlMessages/NoOperation.xml", MessageClass.NoOperation);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void PartialProblemsMessageValidation()
        {
            var validationResult = Validate("XmlMessages/PartialProblems.xml", MessageClass.PartialProblems);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void RegisterMessageValidation()
        {
            var validationResult = Validate("XmlMessages/Register.xml", MessageClass.Register);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void RegisterResponseMessageValidation()
        {
            var validationResult = Validate("XmlMessages/RegisterResponse.xml", MessageClass.RegisterResponse);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void SolutionRequestMessageValidation()
        {
            var validationResult = Validate("XmlMessages/SolutionRequest.xml", MessageClass.SolutionRequest);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void SolutionsMessageValidation()
        {
            var validationResult = Validate("XmlMessages/Solutions.xml", MessageClass.Solutions);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void SolveRequestMessageValidation()
        {
            var validationResult = Validate("XmlMessages/SolveRequest.xml", MessageClass.SolveRequest);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void SolveRequestResponseMessageValidation()
        {
            var validationResult = Validate("XmlMessages/SolveRequestResponse.xml", MessageClass.SolveRequestResponse);
            Assert.IsTrue(validationResult);
        }

        [TestMethod]
        public void StatusMessageValidation()
        {
            var validationResult = Validate("XmlMessages/Status.xml", MessageClass.Status);
            Assert.IsTrue(validationResult);
        }

        private bool Validate(string xmlFilePath, MessageClass typeOfMessageToValidateWith)
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
