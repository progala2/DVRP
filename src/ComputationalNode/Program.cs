using System;
using System.Configuration;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Computations;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Utilities;

namespace _15pl04.Ucc.ComputationalNode
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var appSettings = ConfigurationManager.AppSettings;
            var primaryCSaddress = appSettings["primaryCSaddress"];
            var primaryCSport = appSettings["primaryCSport"];
            var serverAddress = IPEndPointParser.Parse(primaryCSaddress, primaryCSport);
            Console.WriteLine("server address from App.config: " + serverAddress);

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
            }
        }

        private static void computationalNode_OnStarted(object sender, EventArgs e)
        {
            Console.WriteLine("ComputationalNode started.");
        }

        private static void computationalNode_OnStarting(object sender, EventArgs e)
        {
            Console.WriteLine("ComputationalNode is starting...");
        }

        private static void computationalNode_MessageSendingException(object sender, MessageExceptionEventArgs e)
        {
            ColorfulConsole.WriteMessageExceptionInfo("Message sending exception", e.Message, e.Exception);
        }

        private static void computationalNode_MessageHandlingException(object sender, MessageExceptionEventArgs e)
        {
            ColorfulConsole.WriteMessageExceptionInfo("Message handling exception", e.Message, e.Exception);
        }

        private static void computationalNode_MessageEnqueuedToSend(object sender, MessageEventArgs e)
        {
            ColorfulConsole.WriteMessageInfo("Enqueued to send", e.Message);
        }

        private static void computationalNode_MessageReceived(object sender, MessageEventArgs e)
        {
            ColorfulConsole.WriteMessageInfo("Received", e.Message);
        }

        private static void computationalNode_MessageSent(object sender, MessageEventArgs e)
        {
            ColorfulConsole.WriteMessageInfo("Sent", e.Message);
        }
    }
}