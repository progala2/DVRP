namespace _15pl04.Ucc.TaskSolver.Tests
{
    public class TaskSolversLoaderTests
    {
        [Fact]
        public void GetTaskSolversFromRelativePathLoadsTaskSolversCorrectly()
        {
            var taskSolversDictionary = TaskSolverLoader.GetTaskSolversFromRelativePath(@"/TaskSolvers");
            var taskSolversCount = taskSolversDictionary.Keys.Count;
            Assert.True(taskSolversCount > 0);
            Assert.True(taskSolversDictionary.ContainsKey("_15pl04.UCC.MinMax"));

            taskSolversDictionary = TaskSolverLoader.GetTaskSolversFromRelativePath(null);
            taskSolversCount = taskSolversDictionary.Keys.Count;
            Assert.True(taskSolversCount == 0);
        }
    }
}