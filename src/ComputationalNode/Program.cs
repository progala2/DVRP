using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Computations;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.ComputationalNode
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

            var computationalNode = new ComputationalNode(serverAddress, taskSolversDirectoryRelativePath);

            computationalNode.MessageEnqueuedToSend += computationalNode_MessageEnqueuedToSend;
            computationalNode.MessageSent += computationalNode_MessageSent;
            computationalNode.MessageReceived += computationalNode_MessageReceived;
            computationalNode.MessageHandlingException += computationalNode_MessageHandlingException;
            computationalNode.MessageSendingException += computationalNode_MessageSendingException;

            computationalNode.OnStarting += computationalNode_OnStarting;
            computationalNode.OnStarted += computationalNode_OnStarted;
            computationalNode.OnStopping += computationalNode_OnStopping;
            computationalNode.OnStopped += computationalNode_OnStopped;

            computationalNode.Start();
            string line;
            while ((line = Console.ReadLine()) != "exit")
            {
                // input handling
                if (line == "start")
                    computationalNode.Start();
                if (line == "stop")
                    computationalNode.Stop();
            }
            computationalNode.Stop();
        }

        static void computationalNode_OnStopping(object sender, EventArgs e)
        {
            Console.WriteLine("ComputationalNode is stopping...");
        }

        static void computationalNode_OnStopped(object sender, EventArgs e)
        {
            Console.WriteLine("ComputationalNode stopped.");
            _stopwatch.Stop();
            var elapsedTime = _stopwatch.ElapsedMilliseconds / 1000.0;
            Console.WriteLine("Statistics:");
            Console.WriteLine(" Elapsed time: {0}", elapsedTime);
            Console.WriteLine(" Messages sended:   {0}\t{1}/sec", messagesSent, messagesSent / elapsedTime);
            Console.WriteLine(" Messages received: {0}\t{1}/sec", messagesReceived, messagesReceived / elapsedTime);
        }

        static void computationalNode_OnStarted(object sender, EventArgs e)
        {
            Console.WriteLine("ComputationalNode started.");
            messagesSent = messagesReceived = 0;
            _stopwatch.Start();
        }

        static void computationalNode_OnStarting(object sender, EventArgs e)
        {
            Console.WriteLine("ComputationalNode is starting...");
        }

        static void computationalNode_MessageSendingException(object sender, MessageExceptionEventArgs e)
        {
            ColorfulConsole.WriteMessageExceptionInfo("Message sending exception", e.Message, e.Exception);
        }

        static void computationalNode_MessageHandlingException(object sender, MessageExceptionEventArgs e)
        {
            ColorfulConsole.WriteMessageExceptionInfo("Message handling exception", e.Message, e.Exception);
        }

        static void computationalNode_MessageEnqueuedToSend(object sender, MessageEventArgs e)
        {
            ColorfulConsole.WriteMessageInfo("Enqueued to send", e.Message);
        }

        static void computationalNode_MessageReceived(object sender, MessageEventArgs e)
        {
            ColorfulConsole.WriteMessageInfo("Received", e.Message);
            Interlocked.Increment(ref messagesReceived);
        }

        static void computationalNode_MessageSent(object sender, MessageEventArgs e)
        {
            ColorfulConsole.WriteMessageInfo("Sent", e.Message);
            Interlocked.Increment(ref messagesSent);
        }
    }
}
