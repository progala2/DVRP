using System;
using System.Collections.Generic;
using System.Linq;

namespace _15pl04.Ucc.TaskSolver
{
    /// <summary>
    /// Class containing necessary information about a DVRP problem instance.
    /// </summary>
    [Serializable]
    public class DvrpProblem
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="vehicleCount">Number of vehicles.</param>
        /// <param name="vehicleCapacity">Capacity of the vehicles.</param>
        /// <param name="depots">List of depots</param>.
        /// <param name="requests">List of requests.</param>
        /// <param name="cutOffTime">When the request is moved to another day. Should be more than 0 and less or equal to 1.</param>
        public DvrpProblem(int vehicleCount, int vehicleCapacity, IEnumerable<Depot> depots,
            IEnumerable<Request> requests, double cutOffTime = 0.5)
        {
            VehicleCount = vehicleCount;
            VehicleCapacity = vehicleCapacity;
            CutOffTime = cutOffTime;
            Depots = depots.ToArray();
            Requests = requests.ToArray();
            foreach (var t in Requests)
            {
                var dx = Depots[0].X - t.X;
                var dy = Depots[0].Y - t.Y;
                if (t.AvailabilityTime >= Depots[0].EndTime*cutOffTime ||
                    t.AvailabilityTime + Math.Sqrt(dx*dx + dy*dy) > Depots[0].EndTime)
                {
                    t.AvailabilityTime = Depots[0].EndTime;
                }
            }
        }

        /// <summary>
        /// Number of vehicles.
        /// </summary>
        public int VehicleCount { get; private set; }
        /// <summary>
        /// When the request is moved to another day. Should be more than 0 and less or equal 1.
        /// </summary>
        public double CutOffTime { get; private set; }
        /// <summary>
        /// Capacity of the vehicles.
        /// </summary>
        public int VehicleCapacity { get; private set; }
        /// <summary>
        /// List of depots.
        /// </summary>
        public Depot[] Depots { get; private set; }
        /// <summary>
        /// List of requests.
        /// </summary>
        public Request[] Requests { get; private set; }
    }
}