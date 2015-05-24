using System;
using System.Net;
using _15pl04.Ucc.Commons.Computations;
using _15pl04.Ucc.Commons.Computations.Base;
using _15pl04.Ucc.Commons.Config;
using _15pl04.Ucc.Commons.Logging;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Utilities;

namespace _15pl04.Ucc.TaskManager
{
    public class Program
    {
        private static ILogger _logger = new ConsoleLogger();

        private static void Main(string[] args)
        {
            TaskManager taskManager;
            try
            {
                ComponentConfigurationSection config = ComponentConfigurationSection.LoadConfig("componentConfig", args);

                IPEndPoint serverAddress = IpEndPointParser.Parse(config.PrimaryServer.Address, config.PrimaryServer.Port);
                string taskSolversDirectoryRelativePath = config.TaskSolversPath;

                _logger.Info("Server address: " + serverAddress);

                ThreadManager threadManager = new ThreadPoolThreadManager();
                taskManager = new TaskManager(threadManager, serverAddress, taskSolversDirectoryRelativePath);
            }
            catch (Exception ex)
            {
                var errorText = string.Format("{0}:{1}", ex.GetType().FullName, ex.Message);
                if (ex.InnerException != null)
                    errorText += string.Format("|({0}:{1})", ex.InnerException.GetType().FullName, ex.InnerException.Message);
                _logger.Error(errorText);
                return;
            }

            taskManager.MessageEnqueuedToSend += taskManager_MessageEnqueuedToSend;
            taskManager.MessageSent += taskManager_MessageSent;
            taskManager.MessageReceived += taskManager_MessageReceived;
            taskManager.MessageHandlingException += taskManager_MessageHandlingException;
            taskManager.MessageSendingException += taskManager_MessageSendingException;

            taskManager.OnStarting += taskManager_OnStarting;
            taskManager.OnStarted += taskManager_OnStarted;


            taskManager.Start();
            string line;
            while (true)
            {
                line = Console.ReadLine().ToLower();
                if (line == "stop" || line == "quit" || line == "exit")
                    break;
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