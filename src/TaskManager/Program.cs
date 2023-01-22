using System;
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
        private static readonly ILogger Logger = new ConsoleLogger();

        private static void Main(string[] args)
        {
            TaskManager taskManager;
            try
            {
                var config = ComponentConfigurationSection.LoadConfig("componentConfig", args);

                var serverAddress = IpEndPointParser.Parse(config.PrimaryServer.Address, config.PrimaryServer.Port);
                var taskSolversDirectoryRelativePath = config.TaskSolversPath;

                Logger.Info("Server address: " + serverAddress);

                ThreadManager threadManager = new ThreadPoolThreadManager();
                taskManager = new TaskManager(threadManager, serverAddress, taskSolversDirectoryRelativePath);
            }
            catch (Exception ex)
            {
                var errorText = $"{ex.GetType().FullName}:{ex.Message}";
                if (ex.InnerException != null)
                    errorText += $"|({ex.InnerException.GetType().FullName}:{ex.InnerException.Message})";
                Logger.Error(errorText);
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
            string? line;
            while (true)
            {
                line = Console.ReadLine()?.ToLower();
                if (line == "stop" || line == "quit" || line == "exit")
                    break;
                if (line == "start")
                    taskManager.Start();
                if (line == "running")
                    Console.WriteLine("TaskManager.IsRunning={0}", taskManager.IsRunning);
            }
        }

        private static void taskManager_OnStarted(object? sender, EventArgs e)
        {
            Logger.Info("TaskManager started.");
        }

        private static void taskManager_OnStarting(object? sender, EventArgs e)
        {
            Logger.Info("TaskManager is starting...");
        }

        private static void taskManager_MessageSendingException(object? sender, MessageExceptionEventArgs e)
        {
            Logger.Error("Message sending exception:\n" + GetMessageExceptionInfo(e));
        }

        private static void taskManager_MessageHandlingException(object? sender, MessageExceptionEventArgs e)
        {
            Logger.Warn("Message handling exception:\n" + GetMessageExceptionInfo(e));
        }

        private static void taskManager_MessageEnqueuedToSend(object? sender, MessageEventArgs e)
        {
            LogMessageInfo("Enqueued to send", e);
        }

        private static void taskManager_MessageReceived(object? sender, MessageEventArgs e)
        {
            LogMessageInfo("Received", e);
        }

        private static void taskManager_MessageSent(object? sender, MessageEventArgs e)
        {
            LogMessageInfo("Sent", e);
        }

        private static void LogMessageInfo(string description, MessageEventArgs e)
        {
            Logger.Info(description + ": [" + e.Message.MessageType + "]");
            Logger.Debug("\t" + e.Message);
        }

        private static string GetMessageExceptionInfo(MessageExceptionEventArgs e)
        {
            var errorInfo = $" Message: {e.Message}\n Exception: {e.Exception.GetType().FullName}\n  {e.Exception.Message}";
            return errorInfo;
        }
    }
}