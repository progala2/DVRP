using System;

namespace _15pl04.Ucc.TaskSolver
{
    [Serializable]
    public class DvrpSolution
    {
        public double FinalTime { get; private set; }
        public int[][] CarsRoutes { get; private set; }
        public DvrpSolution(double finalTime, int[][] carsRoutes)
        {
            FinalTime = finalTime;
            CarsRoutes = carsRoutes;
        }
    }
}
