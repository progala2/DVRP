using System;
using System.Linq;

namespace _15pl04.Ucc.MinMaxTaskSolver
{
    [Serializable]
    public class MmProblem
    {
	    public MmProblem(int[] numbers)
	    {
		    Numbers = numbers.ToArray();
	    }

        public int[] Numbers { get; private set; }
    }
}