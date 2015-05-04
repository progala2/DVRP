using System;
using System.Configuration;
using System.Net;
using _15pl04.Ucc.Commons.Computations;
using _15pl04.Ucc.Commons.Logging;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Utilities;

namespace _15pl04.Ucc.ComputationalNode
{
    public class Program
    {
        private static ILogger _logger = new TraceSourceLogger("ComputationalNode");

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
            var computationalNode = new ComputationalNode(threadManager, serverAddress, taskSolversDirectoryRelativePath);

            computationalNode.MessageEnqueuedToSend += computationalNode_MessageEnqueuedToSend;
            computationalNode.MessageSent += computationalNode_MessageSent;
            computationalNode.MessageReceived += computationalNode_MessageReceived;
            computationalNode.MessageHandlingException += computationalNode_MessageHandlingException;
            computationalNode.MessageSendingException += computationalNode_MessageSendingException;

            computationalNode.OnStarting += computationalNode_OnStarting;
            computationalNode.OnStarted += computationalNode_OnStarted;

            computationalNode.Start();
            string line;
            while ((line = Console.ReadLine()) != "exit")
            {
                // input handling
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
            string errorInfo = string.Format(" Message: {0}\n Exception: {1}\n  {2}",
                   e.Message, e.Exception.GetType().FullName, e.Exception.Message);
            return errorInfo;
        }
    }
}