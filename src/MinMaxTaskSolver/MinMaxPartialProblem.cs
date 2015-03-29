using System;
using System.Collections.Generic;
using System.Linq;

namespace MinMaxTaskSolver
{
    [Serializable]
    public class MinMaxPartialProblem
    {
        public int[] Numbers { get; private set; }

        public MinMaxPartialProblem(IEnumerable<int> numbers)
        {
            Numbers = numbers.ToArray<int>();
        }
    }
}
