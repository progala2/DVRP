using System;

namespace _15pl04.Ucc.TaskSolver
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DvrpPartialProblem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="setStart"></param>
        /// <param name="approximateResult"></param>
        /// <param name="numberOfSets"></param>
        /// <param name="setEnd"></param>
        public DvrpPartialProblem(int[] setStart, double approximateResult, ulong numberOfSets, int[] setEnd)
        {
            SetBegin = (int[]) setStart.Clone();
            ApproximateResult = approximateResult;
            NumberOfSets = numberOfSets;
            SetEnd = (int[])setEnd.Clone();
        }

        /// <summary>
        /// 
        /// </summary>
        public int[] SetBegin { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public int[] SetEnd { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public ulong NumberOfSets { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public double ApproximateResult { get; private set; }
    }
}