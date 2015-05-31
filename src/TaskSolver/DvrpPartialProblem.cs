using System;

namespace _15pl04.Ucc.TaskSolver
{
    /// <summary>
    /// Class containing necessery information about a partial DVRP.
    /// </summary>
    [Serializable]
    public class DvrpPartialProblem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="setStart">The begining set.</param>
        /// <param name="approximateResult">Information about the best known approximate result of the problem.</param>
        /// <param name="numberOfSets">The number of sets</param>
        /// <param name="setEnd">The ending set.</param>
        public DvrpPartialProblem(int[] setStart, double approximateResult, ulong numberOfSets, int[] setEnd)
        {
            SetBegin = (int[]) setStart.Clone();
            ApproximateResult = approximateResult;
            NumberOfSets = numberOfSets;
            SetEnd = (int[])setEnd.Clone();
        }

        /// <summary>
        /// The begining set.
        /// </summary>
        public int[] SetBegin { get; private set; }
        /// <summary>
        /// The ending set.
        /// </summary>
        public int[] SetEnd { get; private set; }
        /// <summary>
        /// The number of sets
        /// </summary>
        public ulong NumberOfSets { get; private set; }
        /// <summary>
        /// Information about the best known approximate result of the problem.
        /// </summary>
        public double ApproximateResult { get; private set; }
    }
}