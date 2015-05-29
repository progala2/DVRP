using System;
using System.Collections.Generic;
using System.Linq;

namespace _15pl04.Ucc.TaskSolver
{
    [Serializable]
    public class DvrpProblem
    {
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

        public int VehicleCount { get; private set; }
        public double CutOffTime { get; private set; }
        public int VehicleCapacity { get; private set; }
        public Depot[] Depots { get; private set; }
        public Request[] Requests { get; private set; }
    }
}