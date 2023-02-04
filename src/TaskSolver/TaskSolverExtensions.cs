using System;

namespace Dvrp.Ucc.TaskSolver
{
    /// <summary>
    /// Extension methods for the TaskSolve class.
    /// </summary>
    public static class TaskSolverExtensions
    {
        /// <summary>
        /// Throws an exception occurred in task solver if it is in error state.
        /// </summary>
        /// <param name="taskSolver">Extended task solver instance.</param>
        public static void ThrowIfError(this Ucc.TaskSolver.TaskSolver taskSolver)
        {
            if (taskSolver.State == Ucc.TaskSolver.TaskSolverState.Error)
                throw taskSolver.Exception ?? new Exception("Unidentified error in task solver.");
        }
    }
}