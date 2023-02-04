using System;

namespace Dvrp.Ucc.MinMaxTaskSolver
{
    [Serializable]
    public class MmSolution
    {
        public MmSolution(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public int Min { get; private set; }
        public int Max { get; private set; }
    }
}