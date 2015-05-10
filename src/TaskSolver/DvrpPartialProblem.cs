using System;
using System.Collections.Generic;

namespace _15pl04.Ucc.TaskSolver
{
    [Serializable]
    public class DvrpPartialProblem
    {
        public DvrpPartialProblem(List<int[]> sets, double approximateResult)
        {
            Sets = sets;
            ApproximateResult = approximateResult;
        }

        public List<int[]> Sets { get; private set; }
        public double ApproximateResult { get; private set; }
    }
}
