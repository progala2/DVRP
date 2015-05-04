using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Marshalling;
using _15pl04.Ucc.Commons.Messaging.Marshalling.Base;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class ValidationTests
    {
        private readonly IXmlValidator<MessageClass> _validator;

        public ValidationTests()
        {
            _validator = new MessageValidator();
        }

        [TestMethod]
        public void ExampleDivideProblemMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.DivideProblem.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.DivideProblem, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExampleErrorMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.Error.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.Error, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExampleNoOperationMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.NoOperation.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.NoOperation, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExamplePartialProblemsMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.SolvePartialProblems.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.SolvePartialProblems, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExampleRegisterMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.Register.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.Register, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExampleRegisterResponseMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.RegisterResponse.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.RegisterResponse, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExampleSolutionRequestMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.SolutionRequest.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.SolutionRequest, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExampleSolutionsMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.Solutions.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.Solutions, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExampleSolveRequestMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.SolveRequest.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.SolveRequest, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExampleSolveRequestResponseMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.SolveRequestResponse.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.SolveRequestResponse, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExampleStatusMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.Status.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.Status, xmlDoc);

            Assert.IsTrue(passed);
        }
    }
}