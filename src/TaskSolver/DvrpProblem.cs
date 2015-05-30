using System;
using System.Collections.Generic;
using System.Linq;

namespace _15pl04.Ucc.TaskSolver
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DvrpProblem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vehicleCount"></param>
        /// <param name="vehicleCapacity"></param>
        /// <param name="depots"></param>
        /// <param name="requests"></param>
        /// <param name="cutOffTime"></param>
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
        /// 
        /// </summary>
        public int VehicleCount { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public double CutOffTime { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public int VehicleCapacity { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public Depot[] Depots { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public Request[] Requests { get; private set; }
    }
}