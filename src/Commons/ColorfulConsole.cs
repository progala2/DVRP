using System;
using _15pl04.Ucc.Commons.Messaging.Models;

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
                case Message.MessageClassType.Status: consoleColor = ConsoleColor.Cyan; break;
                case Message.MessageClassType.NoOperation: consoleColor = ConsoleColor.DarkCyan; break;
                case Message.MessageClassType.Register: consoleColor = ConsoleColor.Green; break;
                case Message.MessageClassType.RegisterResponse: consoleColor = ConsoleColor.DarkGreen; break;
                case Message.MessageClassType.Error: consoleColor = ConsoleColor.Red; break;
                case Message.MessageClassType.SolveRequest: consoleColor = ConsoleColor.Magenta; break;
                case Message.MessageClassType.SolveRequestResponse: consoleColor = ConsoleColor.DarkMagenta; break;
                case Message.MessageClassType.SolutionRequest: consoleColor = ConsoleColor.Yellow; break;
                case Message.MessageClassType.Solutions: consoleColor = ConsoleColor.DarkYellow; break;
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
