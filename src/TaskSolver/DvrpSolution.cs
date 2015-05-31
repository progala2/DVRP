using System;

namespace _15pl04.Ucc.TaskSolver
{
    /// <summary>
    /// Class containing necessery information about a DVRP solution.
    /// </summary>
    [Serializable]
    public class DvrpSolution
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="finalDistance">The best length of the routes.</param>
        /// <param name="carsRoutes">The routes of the cars.</param>
        public DvrpSolution(double finalDistance, int[][] carsRoutes)
        {
            FinalDistance = finalDistance;
            CarsRoutes = carsRoutes;
        }

        /// <summary>
        /// The best length of the routes.
        /// </summary>
        public double FinalDistance { get; private set; }
        /// <summary>
        /// The routes of the cars.
        /// </summary>
        public int[][] CarsRoutes { get; private set; }
    }
}