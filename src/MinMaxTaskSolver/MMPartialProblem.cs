using System;
using System.Linq;

namespace Dvrp.Ucc.MinMaxTaskSolver
{
    [Serializable]
    public class MmPartialProblem
    {
        public MmPartialProblem(int[] numbers)
        {
            Numbers = numbers.ToArray();
        }

        public int[] Numbers { get; private set; }
    }
}