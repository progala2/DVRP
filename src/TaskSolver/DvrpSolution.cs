using System;

namespace _15pl04.Ucc.TaskSolver
{
    [Serializable]
    public class DvrpSolution
    {
        public double FinalTime { get; private set; }
        public DvrpSolution(double finalTime)
        {
            FinalTime = finalTime;
        }
    }
}
