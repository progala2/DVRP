using System;
using UCCTaskSolver;

namespace _15pl04.Ucc.Commons.Utilities
{
    /// <summary>
    /// Extension methods for the TaskSolve class.
    /// </summary>
    public static class TaskSolverExtensions
    {
        /// <summary>
        /// Throws an exception occured in task solver if it is in error state.
        /// </summary>
        /// <param name="taskSolver">Extended task solver instance.</param>
        public static void ThrowIfError(this TaskSolver taskSolver)
        {
            if (taskSolver.State == TaskSolver.TaskSolverState.Error)
                throw taskSolver.Exception ?? new Exception("Unidentified error in task solver.");
        }
    }
}