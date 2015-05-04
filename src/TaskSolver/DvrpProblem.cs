using System;
using System.Collections.Generic;
using System.Linq;

namespace _15pl04.Ucc.TaskSolver
{
    [Serializable]
    public class DvrpProblem
    {
        public int VehicleCount { get; private set; }
        public double CutOffTime { get; private set; }
        public int VehicleCapacity { get; private set; }
        public Depot[] Depots { get; private set; }
        public Request[] Requests { get; private set; }
        public DvrpProblem(int vehicleCount, int vehicleCapacity, IEnumerable<Depot> depots, IEnumerable<Request> requests, double cutOffTime = 0.5)
        {
            VehicleCount = vehicleCount;
            VehicleCapacity = vehicleCapacity;
            CutOffTime = cutOffTime;
            Depots = depots.ToArray();
            Requests = requests.ToArray();
        }
    }
}
