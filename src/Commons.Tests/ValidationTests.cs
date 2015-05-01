using _15pl04.Ucc.Commons.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using _15pl04.Ucc.Commons.Messaging.Marshalling;
using _15pl04.Ucc.Commons.Messaging.Marshalling.Base;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class ValidationTests
    {
        private IXmlValidator<MessageClass> _validator;

        public ValidationTests()
        {
            _validator = new MessageValidator();
        }

        [TestMethod]
        public void ExampleDivideProblemMessageXmlPassesValidation()
        {
            XDocument xmlDoc = XDocument.Parse(MessageClass.DivideProblem.GetXmlSchema());
            bool passed = _validator.Validate(MessageClass.DivideProblem, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExampleErrorMessageXmlPassesValidation()
        {
            XDocument xmlDoc = XDocument.Parse(MessageClass.Error.GetXmlSchema());
            bool passed = _validator.Validate(MessageClass.Error, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExampleNoOperationMessageXmlPassesValidation()
        {
            XDocument xmlDoc = XDocument.Parse(MessageClass.NoOperation.GetXmlSchema());
            bool passed = _validator.Validate(MessageClass.NoOperation, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExamplePartialProblemsMessageXmlPassesValidation()
        {
            XDocument xmlDoc = XDocument.Parse(MessageClass.SolvePartialProblems.GetXmlSchema());
            bool passed = _validator.Validate(MessageClass.SolvePartialProblems, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExampleRegisterMessageXmlPassesValidation()
        {
            XDocument xmlDoc = XDocument.Parse(MessageClass.Register.GetXmlSchema());
            bool passed = _validator.Validate(MessageClass.Register, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExampleRegisterResponseMessageXmlPassesValidation()
        {
            XDocument xmlDoc = XDocument.Parse(MessageClass.RegisterResponse.GetXmlSchema());
            bool passed = _validator.Validate(MessageClass.RegisterResponse, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExampleSolutionRequestMessageXmlPassesValidation()
        {
            XDocument xmlDoc = XDocument.Parse(MessageClass.SolutionRequest.GetXmlSchema());
            bool passed = _validator.Validate(MessageClass.SolutionRequest, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExampleSolutionsMessageXmlPassesValidation()
        {
            XDocument xmlDoc = XDocument.Parse(MessageClass.Solutions.GetXmlSchema());
            bool passed = _validator.Validate(MessageClass.Solutions, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExampleSolveRequestMessageXmlPassesValidation()
        {
            XDocument xmlDoc = XDocument.Parse(MessageClass.SolveRequest.GetXmlSchema());
            bool passed = _validator.Validate(MessageClass.SolveRequest, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExampleSolveRequestResponseMessageXmlPassesValidation()
        {
            XDocument xmlDoc = XDocument.Parse(MessageClass.SolveRequestResponse.GetXmlSchema());
            bool passed = _validator.Validate(MessageClass.SolveRequestResponse, xmlDoc);

            Assert.IsTrue(passed);
        }

        [TestMethod]
        public void ExampleStatusMessageXmlPassesValidation()
        {
            XDocument xmlDoc = XDocument.Parse(MessageClass.Status.GetXmlSchema());
            bool passed = _validator.Validate(MessageClass.Status, xmlDoc);

            Assert.IsTrue(passed);
        }
    }
}
