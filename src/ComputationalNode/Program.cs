using System;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Computations;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.ComputationalNode
{
    public class Program
    {
        static void Main(string[] args)
        {
            var serverAddress = IPEndPointParser.Parse("127.0.0.1:12345");
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
        }

        static void computationalNode_OnStarted(object sender, EventArgs e)
        {
            Console.WriteLine("ComputationalNode started.");
        }

        static void computationalNode_OnStarting(object sender, EventArgs e)
        {
            Console.WriteLine("ComputationalNode is starting...");
        }

        static void computationalNode_MessageSendingException(object sender, MessageExceptionEventArgs e)
        {
            Console.WriteLine("Message sending exception:");
            Console.WriteLine(" Message: " + e.Message.GetType().Name);
            Console.WriteLine(" Exception: " + e.Exception.GetType() + "\n  " + e.Exception.Message);
        }

        static void computationalNode_MessageHandlingException(object sender, MessageExceptionEventArgs e)
        {
            Console.WriteLine("Message handling exception:");
            Console.WriteLine(" Message: " + e.Message.GetType().Name);
            Console.WriteLine(" Exception: " + e.Exception.GetType());
        }

        static void computationalNode_MessageEnqueuedToSend(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Enqueued to send: " + e.Message.GetType().Name);
        }

        static void computationalNode_MessageReceived(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Received: " + e.Message.GetType().Name);
            if (e.Message.MessageType == Message.MessageClassType.RegisterResponse)
            {
                var msg = (RegisterResponseMessage)e.Message;
                Console.WriteLine(" ID: " + msg.Id);
                Console.WriteLine(" Timeout: " + msg.Timeout);
            }
        }

        static void computationalNode_MessageSent(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Sent: " + e.Message.GetType().Name);
        }
    }
}
