using System.Xml.Linq;
using Dvrp.Ucc.Commons.Messaging;
using Dvrp.Ucc.Commons.Messaging.Marshalling;
using Dvrp.Ucc.Commons.Messaging.Marshalling.Base;
using Xunit;

namespace Dvrp.Ucc.Commons.Tests
{
    public class ValidationTests
    {
        private readonly IXmlValidator<MessageClass> _validator;

        public ValidationTests()
        {
            _validator = new MessageValidator();
        }

        [Fact]
        public void ExampleDivideProblemMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.DivideProblem.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.DivideProblem, xmlDoc);

            Assert.True(passed);
        }

        [Fact]
        public void ExampleErrorMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.Error.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.Error, xmlDoc);

            Assert.True(passed);
        }

        [Fact]
        public void ExampleNoOperationMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.NoOperation.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.NoOperation, xmlDoc);

            Assert.True(passed);
        }

        [Fact]
        public void ExamplePartialProblemsMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.SolvePartialProblems.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.SolvePartialProblems, xmlDoc);

            Assert.True(passed);
        }

        [Fact]
        public void ExampleRegisterMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.Register.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.Register, xmlDoc);

            Assert.True(passed);
        }

        [Fact]
        public void ExampleRegisterResponseMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.RegisterResponse.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.RegisterResponse, xmlDoc);

            Assert.True(passed);
        }

        [Fact]
        public void ExampleSolutionRequestMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.SolutionRequest.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.SolutionRequest, xmlDoc);

            Assert.True(passed);
        }

        [Fact]
        public void ExampleSolutionsMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.Solutions.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.Solutions, xmlDoc);

            Assert.True(passed);
        }

        [Fact]
        public void ExampleSolveRequestMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.SolveRequest.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.SolveRequest, xmlDoc);

            Assert.True(passed);
        }

        [Fact]
        public void ExampleSolveRequestResponseMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.SolveRequestResponse.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.SolveRequestResponse, xmlDoc);

            Assert.True(passed);
        }

        [Fact]
        public void ExampleStatusMessageXmlPassesValidation()
        {
            var xmlDoc = XDocument.Parse(MessageClass.Status.GetXmlSchema());
            var passed = _validator.Validate(MessageClass.Status, xmlDoc);

            Assert.True(passed);
        }
    }
}