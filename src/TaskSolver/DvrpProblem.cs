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
            for (int i = 0; i < Requests.Length; i++)
            {
                double dx = Depots[0].X - Requests[i].X;
                double dy = Depots[0].Y - Requests[i].Y;
                if (Requests[i].AvailabilityTime >= Depots[0].EndTime * cutOffTime || Requests[i].AvailabilityTime + Math.Sqrt(dx * dx + dy * dy) > Depots[0].EndTime)
                {
                    Requests[i].AvailabilityTime = Depots[0].EndTime;
                }
            }
        }
    }
}
