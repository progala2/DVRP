using System;
using UCCTaskSolver;

namespace _15pl04.Ucc.Commons.Utilities
{
    public static class TaskSolverExtensions
    {
        public static void ThrowIfError(this TaskSolver taskSolver)
        {
            if (taskSolver.State == TaskSolver.TaskSolverState.Error)
                throw taskSolver.Exception ?? new Exception("Unidentified error in task solver.");
        }
    }
}