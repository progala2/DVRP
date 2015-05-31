using System;

namespace _15pl04.Ucc.TaskSolver
{
    /// <summary>
    /// Class representing a DVRP solution.
    /// </summary>
    [Serializable]
    public class DvrpSolution
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="finalDistance">The best length of the routes.</param>
        /// <param name="carsRoutes">Cars' routes.</param>
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
        /// Cars' routes.
        /// </summary>
        public int[][] CarsRoutes { get; private set; }
    }
}