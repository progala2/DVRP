using System;
using System.Collections.Generic;
using System.Linq;

namespace _15pl04.Ucc.TaskSolver
{
    [Serializable]
    public class DvrpProblem
    {
        public int VehicleCount { get; private set; }
        public int VehicleCapacity { get; private set; }
        public Depot[] Depots { get; private set; }
        public Request[] Requests { get; private set; }
        public DvrpProblem(int vehicleCount, int vehicleCapacity, IEnumerable<Depot> depots, IEnumerable<Request> requests)
        {
            VehicleCount = vehicleCount;
            VehicleCapacity = vehicleCapacity;
            Depots = depots.ToArray();
            Requests = requests.ToArray();
        }
    }
}
