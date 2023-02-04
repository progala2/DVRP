using System;
using System.Linq;

namespace Dvrp.Ucc.MinMaxTaskSolver
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