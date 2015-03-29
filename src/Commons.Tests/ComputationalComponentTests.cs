using System;
using System.Net;
using _15pl04.Ucc.Commons.Computations;
using _15pl04.Ucc.Commons.Messaging.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class ComputationalComponentTests
    {
        [TestMethod]
        public void ComputationalComponentLoadsCorrectlyTaskSolversFromGivenDirectory()
        {
            var computationalComponent = new ComputationalComponentStub(null, @"/TaskSolvers");
            Assert.IsTrue(computationalComponent.TaskSolversCount() > 0);
        }

        private class ComputationalComponentStub : ComputationalComponent
        {
            public ComputationalComponentStub(IPEndPoint serverAddress)
                : base(serverAddress)
            {
            }
            public ComputationalComponentStub(IPEndPoint serverAddress, string taskSolversDirectoryRelativePath)
                : base(serverAddress, taskSolversDirectoryRelativePath)
            {
            }

            protected override RegisterMessage GetRegisterMessage()
            {
                throw new NotImplementedException();
            }

            protected override void HandleResponseMessage(Message message)
            {
                throw new NotImplementedException();
            }

            public int TaskSolversCount()
            {
                return TaskSolvers.Count;
            }
        }

    }
}
