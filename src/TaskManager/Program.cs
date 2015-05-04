using System;
using System.Configuration;
using System.Net;
using _15pl04.Ucc.Commons.Computations;
using _15pl04.Ucc.Commons.Logging;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Utilities;

namespace _15pl04.Ucc.TaskManager
{
    public class Program
    {
        private static ILogger _logger = new TraceSourceLogger("TaskManager");

        private static void Main(string[] args)
        {
            var appSettings = ConfigurationManager.AppSettings;
            string primaryCSaddress;
            string primaryCSport;
            IPEndPoint serverAddress;
            try
            {
                primaryCSaddress = appSettings["primaryCSaddress"];
                primaryCSport = appSettings["primaryCSport"];
                serverAddress = IpEndPointParser.Parse(primaryCSaddress, primaryCSport);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.ToString());
                return;
            }

            _logger.Info("Server address from App.config: " + serverAddress);

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
                    taskManager.Start();
                if (line == "running")
                    Console.WriteLine("TaskManager.IsRunning={0}", taskManager.IsRunning);
            }
        }

        private static void taskManager_OnStarted(object sender, EventArgs e)
        {
            _logger.Info("TaskManager started.");
        }

        private static void taskManager_OnStarting(object sender, EventArgs e)
        {
            _logger.Info("TaskManager is starting...");
        }

        private static void taskManager_MessageSendingException(object sender, MessageExceptionEventArgs e)
        {
            _logger.Error("Message sending exception:\n" + GetMessageExceptionInfo(e));
        }

        private static void taskManager_MessageHandlingException(object sender, MessageExceptionEventArgs e)
        {
            _logger.Warn("Message handling exception:\n" + GetMessageExceptionInfo(e));
        }

        private static void taskManager_MessageEnqueuedToSend(object sender, MessageEventArgs e)
        {
            LogMessageInfo("Enqueued to send", e);
        }

        private static void taskManager_MessageReceived(object sender, MessageEventArgs e)
        {
            LogMessageInfo("Received", e);
        }

        private static void taskManager_MessageSent(object sender, MessageEventArgs e)
        {
            LogMessageInfo("Sent", e);
        }

        private static void LogMessageInfo(string description, MessageEventArgs e)
        {
            _logger.Info(description + ": [" + e.Message.MessageType + "]");
            _logger.Debug("\t" + e.Message);
        }

        private static string GetMessageExceptionInfo(MessageExceptionEventArgs e)
        {
            string errorInfo = string.Format(" Message: {0}\n Exception: {1}\n  {2}",
                   e.Message, e.Exception.GetType().FullName, e.Exception.Message);
            return errorInfo;
        }

    }
}