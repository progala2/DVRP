using System;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Computations;
using _15pl04.Ucc.Commons.Messaging;

namespace _15pl04.Ucc.TaskManager
{
    public class Program
    {
        static void Main(string[] args)
        {
            var serverAddress = IPEndPointParser.Parse("127.0.0.1:12345");
            var taskSolversDirectoryRelativePath = @""; // current directory

            var taskManager = new TaskManager(serverAddress, taskSolversDirectoryRelativePath);

            taskManager.MessageEnqueuedToSend += taskManager_MessageEnqueuedToSend;
            taskManager.MessageHandlingException += taskManager_MessageHandlingException;
            taskManager.MessageSended += taskManager_MessageSended;
            taskManager.MessageReceived += taskManager_MessageReceived;


            try
            {
                taskManager.Start();
            }
            catch (Commons.Exceptions.RegisterException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            while (Console.ReadLine() != "exit")
            {
                // input handling
            }
            taskManager.Stop();
        }

        static void taskManager_MessageHandlingException(object sender, MessageHandlingExceptionEventArgs e)
        {
            Console.WriteLine("Message handling exception:");
            Console.WriteLine(" Message:" + e.Message.MessageType + "Message");
            Console.WriteLine(" Exception:" + e.Exception.Message);
        }

        static void taskManager_MessageEnqueuedToSend(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Enqueued to send: " + e.Message.MessageType + "Message");
        }

        static void taskManager_MessageReceived(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Received: " + e.Message.MessageType + "Message");
        }

        static void taskManager_MessageSended(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Sended: " + e.Message.MessageType + "Message");
        }
    }
}
