using System;
using System.Collections.Generic;
using System.Linq;

namespace _15pl04.Ucc.MinMaxTaskSolver
{
    [Serializable]
    public class MMProblem
    {
        public MMProblem(IEnumerable<int> numbers)
        {
            Numbers = numbers.ToArray();
        }

        public int[] Numbers { get; private set; }
    }
}