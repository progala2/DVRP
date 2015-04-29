using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
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
            var appSettings = ConfigurationManager.AppSettings;
            var primaryCSaddress = appSettings["primaryCSaddress"];
            var primaryCSport = appSettings["primaryCSport"];
            var serverAddress = IPEndPointParser.Parse(primaryCSaddress, primaryCSport);
            Console.WriteLine("server address from App.config: " + serverAddress);

            var taskSolversDirectoryRelativePath = @""; // current directory

            var threadManager = new ThreadPoolThreadManager();
            var taskManager = new TaskManager(threadManager, serverAddress, taskSolversDirectoryRelativePath);

            taskManager.MessageEnqueuedToSend += taskManager_MessageEnqueuedToSend;
            taskManager.MessageSent += taskManager_MessageSent;
            taskManager.MessageReceived += taskManager_MessageReceived;
            taskManager.MessageHandlingException += taskManager_MessageHandlingException;
            taskManager.MessageSendingException += taskManager_MessageSendingException;

            taskManager.OnStarting += taskManager_OnStarting;
            taskManager.OnStarted += taskManager_OnStarted;



            taskManager.Start();
            string line;
            while ((line = Console.ReadLine()) != "exit")
            {
                // input handling
                if (line == "start")
                {
                    taskManager.Start();
                }
            }
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
            ColorfulConsole.WriteMessageExceptionInfo("Message sending exception", e.Message, e.Exception);
        }

        static void taskManager_MessageHandlingException(object sender, MessageExceptionEventArgs e)
        {
            ColorfulConsole.WriteMessageExceptionInfo("Message handling exception", e.Message, e.Exception);
        }

        static void taskManager_MessageEnqueuedToSend(object sender, MessageEventArgs e)
        {
            ColorfulConsole.WriteMessageInfo("Enqueued to send", e.Message);
        }

        static void taskManager_MessageReceived(object sender, MessageEventArgs e)
        {
            ColorfulConsole.WriteMessageInfo("Received", e.Message);
        }

        static void taskManager_MessageSent(object sender, MessageEventArgs e)
        {
            ColorfulConsole.WriteMessageInfo("Sent", e.Message);
        }
    }
}
