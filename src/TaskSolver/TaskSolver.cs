using System;

namespace _15pl04.Ucc.TaskSolver
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class TaskSolver
    {
        /// <summary>
        /// 
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Divide problem into smaller ones.
        /// </summary>
        /// <param name="threadCount">Number of divisions.</param>
        /// <returns>Binary serialized parts of the problem.</returns>
        public abstract byte[][]? DivideProblem(int threadCount);

        /// <summary>
        /// 
        /// </summary>
        public TaskSolverState State { get; protected set; }
        /// <summary>
        /// 
        /// </summary>
        public Exception Exception { get; protected set; }

        /// <summary>
        /// Merge partial solutions into the final one.
        /// </summary>
        /// <param name="solutions">Binary serialized DVRP solutions <see cref="DvrpSolution"/>.</param>
        /// <returns>Binary serialized string with a description of the best solution.</returns>
        public abstract byte[]? MergeSolution(byte[][] solutions);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partialData"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public abstract byte[]? Solve(byte[] partialData, TimeSpan timeout);
    }
}
