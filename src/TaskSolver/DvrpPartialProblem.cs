using System;

namespace _15pl04.Ucc.TaskSolver
{
    [Serializable]
    public class DvrpPartialProblem
    {
        public DvrpPartialProblem(int[] setStart, double approximateResult, ulong numberOfSets, int[] setEnd)
        {
            SetBegin = (int[]) setStart.Clone();
            ApproximateResult = approximateResult;
            NumberOfSets = numberOfSets;
            SetEnd = (int[])setEnd.Clone();
        }

        public int[] SetBegin { get; private set; }
        public int[] SetEnd { get; private set; }
        public ulong NumberOfSets { get; private set; }
        public double ApproximateResult { get; private set; }
    }
}