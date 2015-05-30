using System;

namespace _15pl04.Ucc.TaskSolver
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Depot
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        public Depot(double x, double y, double startTime, double endTime)
        {
            X = x;
            Y = y;
            StartTime = startTime;
            EndTime = endTime;
        }

        /// <summary>
        /// 
        /// </summary>
        public double X { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public double Y { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public double StartTime { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public double EndTime { get; private set; }
    }
}