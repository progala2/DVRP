using System;

namespace _15pl04.Ucc.TaskSolver
{
    [Serializable]
    public class DvrpSolution
    {
        public DvrpSolution(double finalTime, int[][] carsRoutes)
        {
            FinalDistance = finalTime;
            CarsRoutes = carsRoutes;
        }

        public double FinalDistance { get; private set; }
        public int[][] CarsRoutes { get; private set; }
    }
}