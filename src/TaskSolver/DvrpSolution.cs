using System;

namespace _15pl04.Ucc.TaskSolver
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DvrpSolution
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="finalTime"></param>
        /// <param name="carsRoutes"></param>
        public DvrpSolution(double finalTime, int[][] carsRoutes)
        {
            FinalDistance = finalTime;
            CarsRoutes = carsRoutes;
        }

        /// <summary>
        /// 
        /// </summary>
        public double FinalDistance { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public int[][] CarsRoutes { get; private set; }
    }
}