using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class TaskSolversLoaderTests
    {
        [TestMethod]
        public void GetTaskSolversFromRelativePathLoadsTaskSolversCorrectly()
        {
            var taskSolversDictionary = TaskSolverLoader.GetTaskSolversFromRelativePath(@"/TaskSolvers");
            var taskSolversCount = taskSolversDictionary.Keys.Count;
            Assert.IsTrue(taskSolversCount > 0);
            Assert.IsTrue(taskSolversDictionary.ContainsKey("_15pl04.UCC.MinMax"));

            taskSolversDictionary = TaskSolverLoader.GetTaskSolversFromRelativePath(null);
            taskSolversCount = taskSolversDictionary.Keys.Count;
            Assert.IsTrue(taskSolversCount == 0);
        }
    }
}