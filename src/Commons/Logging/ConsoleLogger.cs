using System;
using System.Diagnostics;

namespace _15pl04.Ucc.Commons.Logging
{
    /// <summary>
    /// Prints logger messages on the console in an appropriate color.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// Log TRACE level message.
        /// </summary>
        /// <param name="s">Message to log.</param>
        public void Trace(string s)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(GetCallerInfoPrefix() + s);
            Console.ResetColor();
        }

        /// <summary>
        /// Log DEBUG level message.
        /// </summary>
        /// <param name="s">Message to log.</param>
        public void Debug(string s)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(GetCallerInfoPrefix() + s);
            Console.ResetColor();
        }

        /// <summary>
        /// Log INFO level message.
        /// </summary>
        /// <param name="s">Message to log.</param>
        public void Info(string? s)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(GetCallerInfoPrefix() + s);
            Console.ResetColor();
        }

        /// <summary>
        /// Log WARN level message.
        /// </summary>
        /// <param name="s">Message to log.</param>
        public void Warn(string s)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(GetCallerInfoPrefix() + s);
            Console.ResetColor();
        }

        /// <summary>
        /// Log ERROR level message.
        /// </summary>
        /// <param name="s">Message to log.</param>
        public void Error(string? s)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(GetCallerInfoPrefix() + s);
            Console.ResetColor();
        }

        /// <summary>
        /// Gets information about caller class and method as string.
        /// </summary>
        /// <returns>Log message prefix.</returns>
        private static string GetCallerInfoPrefix()
        {
            var frame = new StackFrame(2);
            var method = frame.GetMethod();

            var className = method?.DeclaringType?.Name;
            var methodName = method?.Name;

            return "[" + className + "/" + methodName + "]::";
        }
    }
}