using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.CommunicationServer.Tasks;
using _15pl04.Ucc.CommunicationServer.Tasks.Models;

namespace _15pl04.Ucc.CommunicationServer.Tests
{
    [TestClass]
    public class TaskSchedulerTests
    {
        ProblemInstance problemInstance;
        PartialProblem partialProblem;

        public TaskSchedulerTests()
        {
            problemInstance = new ProblemInstance(123, "dvrp", new byte[] { 4, 5, 6 }, 0);
            partialProblem = new PartialProblem(problemInstance, 789, new byte[] { 0, 9, 8 }, new byte[] { 7, 6, 5 });
        }

        [TestMethod]
        public void InputAndOutputProblemInstanceReferToTheSameObject()
        {
            ProblemInstance inputProblemInstance = problemInstance;
            TaskScheduler.Instance.AddNewProblemInstance(inputProblemInstance);

            ProblemInstance outputProblemInstance;
            ulong taskManagerId = 1337;
            TaskScheduler.Instance.GetProblemInstanceToDivide("dvrp", out outputProblemInstance, taskManagerId);

            Assert.AreSame(inputProblemInstance, outputProblemInstance);
        }

        [TestMethod]
        public void InputAndOutputPartialProblemReferToTheSameObject()
        {
            TaskScheduler.Instance.AddNewProblemInstance(problemInstance);
            ProblemInstance outputProblemInstance;
            TaskScheduler.Instance.GetProblemInstanceToDivide("dvrp", out outputProblemInstance, 1337);

            PartialProblem inputPartialProblem = partialProblem;
            TaskScheduler.Instance.AddPartialProblems(new PartialProblem[] { inputPartialProblem });

            PartialProblem[] outputPartialProblems;
            ulong compNodeId = 7331;
            TaskScheduler.Instance.GetPartialProblemsToCompute("dvrp", 4, out outputPartialProblems, compNodeId);

            Assert.AreSame(inputPartialProblem, outputPartialProblems[0]);
        }


    }
}
