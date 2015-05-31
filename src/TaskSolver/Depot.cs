using System;

namespace _15pl04.Ucc.TaskSolver
{
    /// <summary>
    /// Class containing necessery information about a depot station.
    /// </summary>
    [Serializable]
    public class Depot
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X location coordination.</param>
        /// <param name="y">Y location coordination.</param>
        /// <param name="startTime">When the depot opens. In minutes.</param>
        /// <param name="endTime">When the depot closes. In minutes.</param>
        public Depot(double x, double y, double startTime, double endTime)
        {
            X = x;
            Y = y;
            StartTime = startTime;
            EndTime = endTime;
        }

        /// <summary>
        /// Location coordination.
        /// </summary>
        public double X { get; private set; }
        /// <summary>
        /// Location coordination.
        /// </summary>
        public double Y { get; private set; }
        /// <summary>
        /// When the depot opens. In minutes.
        /// </summary>
        public double StartTime { get; private set; }
        /// <summary>
        /// When the depot closes. In minutes.
        /// </summary>
        public double EndTime { get; private set; }
    }
}