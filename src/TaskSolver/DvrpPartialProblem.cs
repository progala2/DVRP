using System;
using System.Collections.Generic;

namespace _15pl04.Ucc.TaskSolver
{
    [Serializable]
    public class DvrpPartialProblem
    {
        public DvrpPartialProblem(int[] setStart, double approximateResult, ulong numberOfSets)
        {
            SetBegin = (int[])setStart.Clone();
            ApproximateResult = approximateResult;
            NumberOfSets = numberOfSets;
        }

        public int[] SetBegin { get; private set; }
        public ulong NumberOfSets { get; private set; }
        public double ApproximateResult { get; private set; }
    }
}
