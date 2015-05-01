using System;
using System.Diagnostics;

namespace _15pl04.Ucc.Commons.Logging
{
    public class ConsoleLogger : ILogger
    {
        // TODO singleton

        public void Trace(string s)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(GetCallerInfoPrefix() + s);
            Console.ResetColor();
        }

        public void Debug(string s)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(GetCallerInfoPrefix() + s);
            Console.ResetColor();
        }

        public void Info(string s)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(GetCallerInfoPrefix() + s);
            Console.ResetColor();
        }

        public void Warn(string s)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(GetCallerInfoPrefix() + s);
            Console.ResetColor();
        }

        public void Error(string s)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(GetCallerInfoPrefix() + s);
            Console.ResetColor();
        }

        private string GetCallerInfoPrefix()
        {
            var frame = new StackFrame(2);
            var method = frame.GetMethod();

            var className = method.DeclaringType.Name;
            var methodName = method.Name;

            return "[" + className + "/" + methodName + "]::";
        }
    }
}