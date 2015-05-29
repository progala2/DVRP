using System;
using _15pl04.Ucc.Commons.Computations;
using _15pl04.Ucc.Commons.Computations.Base;
using _15pl04.Ucc.Commons.Config;
using _15pl04.Ucc.Commons.Logging;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Utilities;

namespace _15pl04.Ucc.ComputationalNode
{
    public class Program
    {
        private static readonly ILogger _logger = new ConsoleLogger();

        private static void Main(string[] args)
        {
            ComputationalNode computationalNode;
            try
            {
                var config = ComponentConfigurationSection.LoadConfig("componentConfig", args);

                var serverAddress = IpEndPointParser.Parse(config.PrimaryServer.Address, config.PrimaryServer.Port);
                var taskSolversDirectoryRelativePath = config.TaskSolversPath;

                _logger.Info("Server address: " + serverAddress);

                ThreadManager threadManager = new ThreadPoolThreadManager();
                computationalNode = new ComputationalNode(threadManager, serverAddress, taskSolversDirectoryRelativePath);
            }
            catch (Exception ex)
            {
                var errorText = string.Format("{0}:{1}", ex.GetType().FullName, ex.Message);
                if (ex.InnerException != null)
                    errorText += string.Format("|({0}:{1})", ex.InnerException.GetType().FullName,
                        ex.InnerException.Message);
                _logger.Error(errorText);
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
            string line;
            while (true)
            {
                line = Console.ReadLine().ToLower();
                if (line == "stop" || line == "quit" || line == "exit")
                    break;
                if (line == "start")
                    computationalNode.Start();
                if (line == "running")
                    Console.WriteLine("ComputationalNode.IsRunning={0}", computationalNode.IsRunning);
            }
        }

        private static void computationalNode_OnStarted(object sender, EventArgs e)
        {
            _logger.Info("ComputationalNode started.");
        }

        private static void computationalNode_OnStarting(object sender, EventArgs e)
        {
            _logger.Info("ComputationalNode is starting...");
        }

        private static void computationalNode_MessageSendingException(object sender, MessageExceptionEventArgs e)
        {
            _logger.Error("Message sending exception:\n" + GetMessageExceptionInfo(e));
        }

        private static void computationalNode_MessageHandlingException(object sender, MessageExceptionEventArgs e)
        {
            _logger.Warn("Message handling exception:\n" + GetMessageExceptionInfo(e));
        }

        private static void computationalNode_MessageEnqueuedToSend(object sender, MessageEventArgs e)
        {
            LogMessageInfo("Enqueued to send", e);
        }

        private static void computationalNode_MessageReceived(object sender, MessageEventArgs e)
        {
            LogMessageInfo("Received", e);
        }

        private static void computationalNode_MessageSent(object sender, MessageEventArgs e)
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
            var errorInfo = string.Format(" Message: {0}\n Exception: {1}\n  {2}",
                e.Message, e.Exception.GetType().FullName, e.Exception.Message);
            return errorInfo;
        }
    }
}