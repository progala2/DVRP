using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _15pl04.Ucc.TaskSolver
{
    [Serializable]
    public class Depot
    {
        public Depot(float x, float y, float startTime, float endTime)
        {
            X = x;
            Y = y;
            StartTime = startTime;
            EndTime = endTime;
        }

        public float X { get; private set; }
        public float Y { get; private set; }
        public float StartTime { get; private set; }
        public float EndTime { get; private set; }
    }
}
