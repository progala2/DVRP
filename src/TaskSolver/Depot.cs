using System;

namespace _15pl04.Ucc.TaskSolver
{
    /// <summary>
    /// Class containing all necessary information about a depot station.
    /// </summary>
    [Serializable]
    public class Depot
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="x">X coordinate.</param>
        /// <param name="y">Y coordinate.</param>
        /// <param name="startTime">Time the depot opens; in minutes.</param>
        /// <param name="endTime">Closing time; in minutes.</param>
        public Depot(double x, double y, double startTime, double endTime)
        {
            X = x;
            Y = y;
            StartTime = startTime;
            EndTime = endTime;
        }

        /// <summary>
        /// Position of the depot: X coordinate.
        /// </summary>
        public double X { get; private set; }
        /// <summary>
        /// Position of the depot: Y coordinate.
        /// </summary>
        public double Y { get; private set; }
        /// <summary>
        /// Depot opening time; in minutes.
        /// </summary>
        public double StartTime { get; private set; }
        /// <summary>
        /// Depot closing time; in minutes.
        /// </summary>
        public double EndTime { get; private set; }
    }
}