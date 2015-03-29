using System;

namespace MinMaxTaskSolver
{
    [Serializable]
    public class MinMaxSolution
    {
        public int Min { get; private set; }
        public int Max { get; private set; }

        public MinMaxSolution(int min, int max)
        {
            Min = min;
            Max = max;
        }
    }
}
