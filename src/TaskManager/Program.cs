using System;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Computations;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;

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
            taskManager.MessageSent += taskManager_MessageSent;
            taskManager.MessageReceived += taskManager_MessageReceived;
            taskManager.MessageHandlingException += taskManager_MessageHandlingException;
            taskManager.MessageSendingException += taskManager_MessageSendingException;

            taskManager.OnStarting += taskManager_OnStarting;
            taskManager.OnStarted += taskManager_OnStarted;
            taskManager.OnStopping += taskManager_OnStopping;
            taskManager.OnStopped += taskManager_OnStopped;

            taskManager.Start();
            string line;
            while ((line = Console.ReadLine()) != "exit")
            {
                // input handling
                if (line == "start")
                    taskManager.Start();
                if (line == "stop")
                    taskManager.Stop();
            }
            taskManager.Stop();
        }

        static void taskManager_OnStopping(object sender, EventArgs e)
        {
            Console.WriteLine("TaskManager is stopping...");
        }

        static void taskManager_OnStopped(object sender, EventArgs e)
        {
            Console.WriteLine("TaskManager stopped.");
        }

        static void taskManager_OnStarted(object sender, EventArgs e)
        {
            Console.WriteLine("TaskManager started.");
        }

        static void taskManager_OnStarting(object sender, EventArgs e)
        {
            Console.WriteLine("TaskManager is starting...");
        }

        static void taskManager_MessageSendingException(object sender, MessageExceptionEventArgs e)
        {
            Console.WriteLine("Message sending exception:");
            Console.WriteLine(" Message: " + e.Message.GetType().Name);
            Console.WriteLine(" Exception: " + e.Exception.GetType() + "\n  " + e.Exception.Message);
        }

        static void taskManager_MessageHandlingException(object sender, MessageExceptionEventArgs e)
        {
            Console.WriteLine("Message handling exception:");
            Console.WriteLine(" Message: " + e.Message.GetType().Name);
            Console.WriteLine(" Exception: " + e.Exception.GetType());
        }

        static void taskManager_MessageEnqueuedToSend(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Enqueued to send: " + e.Message.GetType().Name);
        }

        static void taskManager_MessageReceived(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Received: " + e.Message.GetType().Name);
            if (e.Message.MessageType == Message.MessageClassType.RegisterResponse)
            {
                var msg = (RegisterResponseMessage)e.Message;
                Console.WriteLine(" ID: " + msg.Id);
                Console.WriteLine(" Timeout: " + msg.Timeout);
            }
        }

        static void taskManager_MessageSent(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Sent: " + e.Message.GetType().Name);
        }
    }
}
