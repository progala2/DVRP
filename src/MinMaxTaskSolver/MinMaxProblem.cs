using System;
using System.Collections.Generic;
using System.Linq;

namespace MinMaxTaskSolver
{
    [Serializable]
    public class MinMaxProblem
    {
        public int[] Numbers { get; private set; }

        public MinMaxProblem(IEnumerable<int> numbers)
        {
            Numbers = numbers.ToArray<int>();
        }
    }
}
