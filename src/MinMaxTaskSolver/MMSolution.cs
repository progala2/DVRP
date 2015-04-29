using System;

namespace _15pl04.Ucc.MinMaxTaskSolver
{
    [Serializable]
    public class MMSolution
    {
        public int Min { get; private set; }
        public int Max { get; private set; }

        public MMSolution(int min, int max)
        {
            Min = min;
            Max = max;
        }
    }
}
