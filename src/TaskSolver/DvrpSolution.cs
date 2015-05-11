using System;

namespace _15pl04.Ucc.TaskSolver
{
    [Serializable]
    public class DvrpSolution
    {
        public double FinalDistance { get; private set; }
        public int[][] CarsRoutes { get; private set; }
        public DvrpSolution(double finalTime, int[][] carsRoutes)
        {
            FinalDistance = finalTime;
            CarsRoutes = carsRoutes;
        }
    }
}
