using System;
using Dvrp.Ucc.Commons.Computations;
using Dvrp.Ucc.Commons.Computations.Base;
using Dvrp.Ucc.Commons.Config;
using Dvrp.Ucc.Commons.Logging;
using Dvrp.Ucc.Commons.Messaging;
using Dvrp.Ucc.Commons.Utilities;

namespace Dvrp.Ucc.ComputationalNode
{
    public class Program
    {
        private static readonly ILogger Logger = new ConsoleLogger();

        private static void Main(string[] args)
        {
            ComputationalNode computationalNode;
            try
            {
                var config = ComponentConfigurationSection.LoadConfig("componentConfig", args);

                var serverAddress = IpEndPointParser.Parse(config.PrimaryServer.Address, config.PrimaryServer.Port);
                var taskSolversDirectoryRelativePath = config.TaskSolversPath;

                Logger.Info("Server address: " + serverAddress);

                ThreadManager threadManager = new ThreadPoolThreadManager();
                computationalNode = new ComputationalNode(threadManager, serverAddress, taskSolversDirectoryRelativePath);
            }
            catch (Exception ex)
            {
                var errorText = $"{ex.GetType().FullName}:{ex.Message}";
                if (ex.InnerException != null)
                    errorText += $"|({ex.InnerException.GetType().FullName}:{ex.InnerException.Message})";
                Logger.Error(errorText);
                return;
            }

            computationalNode.MessageEnqueuedToSend += computationalNode_MessageEnqueuedToSend;
            computationalNode.MessageSent += computationalNode_MessageSent;
            computationalNode.MessageReceived += computationalNode_MessageReceived;
            computationalNode.MessageHandlingException += computationalNode_MessageHandlingException;
            computationalNode.MessageSendingException += computationalNode_MessageSendingException;

            computationalNode.OnStarting += computationalNode_OnStarting;
            computationalNode.OnStarted += computationalNode_OnStarted;

            computationalNode.Start();
            string? line;
            while (true)
            {
                line = Console.ReadLine()?.ToLower();
                if (line == "stop" || line == "quit" || line == "exit")
                    break;
                if (line == "start")
                    computationalNode.Start();
                if (line == "running")
                    Console.WriteLine("ComputationalNode.IsRunning={0}", computationalNode.IsRunning);
            }
        }

        private static void computationalNode_OnStarted(object? sender, EventArgs e)
        {
            Logger.Info("ComputationalNode started.");
        }

        private static void computationalNode_OnStarting(object? sender, EventArgs e)
        {
            Logger.Info("ComputationalNode is starting...");
        }

        private static void computationalNode_MessageSendingException(object? sender, MessageExceptionEventArgs e)
        {
            Logger.Error("Message sending exception:\n" + GetMessageExceptionInfo(e));
        }

        private static void computationalNode_MessageHandlingException(object? sender, MessageExceptionEventArgs e)
        {
            Logger.Warn("Message handling exception:\n" + GetMessageExceptionInfo(e));
        }

        private static void computationalNode_MessageEnqueuedToSend(object? sender, MessageEventArgs e)
        {
            LogMessageInfo("Enqueued to send", e);
        }

        private static void computationalNode_MessageReceived(object? sender, MessageEventArgs e)
        {
            LogMessageInfo("Received", e);
        }

        private static void computationalNode_MessageSent(object? sender, MessageEventArgs e)
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