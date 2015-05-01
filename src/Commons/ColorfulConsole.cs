using System;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Messaging.Models.Base;

namespace _15pl04.Ucc.Commons
{
    public static class ColorfulConsole
    {
        public static void WriteMessageInfo(string description, Message message)
        {
            Console.Write(description + ": ");
            ConsoleColor consoleColor;
            switch (message.MessageType)
            {
                case MessageClass.Status: consoleColor = ConsoleColor.Cyan; break;
                case MessageClass.NoOperation: consoleColor = ConsoleColor.DarkCyan; break;
                case MessageClass.Register: consoleColor = ConsoleColor.Green; break;
                case MessageClass.RegisterResponse: consoleColor = ConsoleColor.DarkGreen; break;
                case MessageClass.Error: consoleColor = ConsoleColor.Red; break;
                case MessageClass.SolveRequest: consoleColor = ConsoleColor.Magenta; break;
                case MessageClass.SolveRequestResponse: consoleColor = ConsoleColor.DarkMagenta; break;
                case MessageClass.SolutionRequest: consoleColor = ConsoleColor.Yellow; break;
                case MessageClass.Solutions: consoleColor = ConsoleColor.DarkYellow; break;
                default: consoleColor = ConsoleColor.Gray; break;
            }
            Console.ForegroundColor = consoleColor;
            Console.WriteLine(message.GetType().Name);
            Console.WriteLine("\t" + message.ToString());
            Console.ResetColor();
        }

        public static void WriteMessageExceptionInfo(string description, Message message, Exception exception)
        {
            WriteMessageInfo(description, message);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\t" + exception.GetType());
            Console.WriteLine("\t " + exception.Message);
            Console.ResetColor();

        }
    }
}
