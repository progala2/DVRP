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
        private static Stopwatch _stopwatch = new Stopwatch();
        private static int messagesSent;
        private static int messagesReceived;

        static void Main(string[] args)
        {
            var appSettings = ConfigurationManager.AppSettings;
            var primaryCSaddress = appSettings["primaryCSaddress"];
            var primaryCSport = appSettings["primaryCSport"];
            var serverAddress = IPEndPointParser.Parse(primaryCSaddress, primaryCSport);
            Console.WriteLine("server address from App.config: " + serverAddress);

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
                {
                    taskManager.Start();
                }
                if (line == "stop")
                {
                    taskManager.Stop();
                }
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
            _stopwatch.Stop();
            var elapsedTime = _stopwatch.ElapsedMilliseconds / 1000.0;
            Console.WriteLine("Statistics:");
            Console.WriteLine(" Elapsed time: {0}", elapsedTime);
            Console.WriteLine(" Messages sended:   {0}\t{1}/sec", messagesSent, messagesSent / elapsedTime);
            Console.WriteLine(" Messages received: {0}\t{1}/sec", messagesReceived, messagesReceived / elapsedTime);
        }

        static void taskManager_OnStarted(object sender, EventArgs e)
        {
            Console.WriteLine("TaskManager started.");
            messagesSent = messagesReceived = 0;
            _stopwatch.Start();
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
            Interlocked.Increment(ref messagesReceived);
        }

        static void taskManager_MessageSent(object sender, MessageEventArgs e)
        {
            ColorfulConsole.WriteMessageInfo("Sent", e.Message);
            Interlocked.Increment(ref messagesSent);
        }
    }
}
