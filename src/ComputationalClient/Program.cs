using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Computations;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using MinMaxTaskSolver;

namespace _15pl04.Ucc.ComputationalClient
{
    public class Program
    {
        static void Main(string[] args)
        {
            var serverAddress = IPEndPointParser.Parse("127.0.0.1:12345");

            var computationalClient = new ComputationalClient(serverAddress);

            computationalClient.MessageSendingException += computationalClient_MessageSendingException;
            computationalClient.MessageReceived += computationalClient_MessageReceived;
            computationalClient.MessageSent += computationalClient_MessageSent;

            var problemType = "UCC.MinMax";

            string line;
            while ((line = Console.ReadLine()) != "exit")
            {
                // input handling
                if (line == "solve")
                {
                    var numbers = GenerateNumbers(10, 0, 50);
                    var minMaxProblem = new MinMaxProblem(numbers);
                    var problemData = GenerateProblemData(minMaxProblem);
                    computationalClient.SendSolveRequest(problemType, problemData, null);
                    
                }
                if (line == "solution")
                {
                    Console.Write("Enter problem id: ");
                    uint id;
                    uint.TryParse(Console.ReadLine(), out id);
                    var solutionsMessage = computationalClient.SendSolutionRequest(id);
                    if (solutionsMessage != null)
                        ShowSolutionsMessageInfo(solutionsMessage);
                }
            }
        }

        static void computationalClient_MessageSent(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Sent: " + e.Message.GetType().Name);
        }

        static void computationalClient_MessageReceived(object sender, MessageEventArgs e)
        {
            Console.WriteLine("Received: " + e.Message.GetType().Name);
            if (e.Message.MessageType == Message.MessageClassType.SolveRequestResponse)
            {
                var msg = (SolveRequestResponseMessage)e.Message;
                Console.WriteLine(" Received problem id=" + msg.Id);
            }
            if (e.Message.MessageType == Message.MessageClassType.Solutions)
            {
                var msg = (SolutionsMessage)e.Message;
                ShowSolutionsMessageInfo(msg);
            }
        }

        static void computationalClient_MessageSendingException(object sender,MessageExceptionEventArgs e)
        {
            Console.WriteLine("Message sending exception:");
            Console.WriteLine(" Message: " + e.Message.GetType().Name);
            Console.WriteLine(" Exception: " + e.Exception.GetType() + "\n  " + e.Exception.Message);
        }

        private static void ShowSolutionsMessageInfo(SolutionsMessage solutionsMessage)
        {
            Console.WriteLine("SolutionsMessage:");
            Console.WriteLine(" ProblemType=" + solutionsMessage.ProblemType);
            Console.WriteLine(" Id=" + solutionsMessage.Id);
            foreach (var solution in solutionsMessage.Solutions)
            {
                Console.WriteLine("  TaskId=" + solution.TaskId);
                Console.WriteLine("  ComputationsTime=" + solution.ComputationsTime);
                Console.WriteLine("  TimeoutOccured=" + solution.TimeoutOccured);
            }
        }

        private static byte[] GenerateProblemData(MinMaxProblem minMaxProblem)
        {
            using (var memoryStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, minMaxProblem);
                var problemData = memoryStream.ToArray();
                return problemData;
            }
        }


        private static List<int> GenerateNumbers(int numbersCount, int min, int max)
        {
            var rand = new Random();
            var result = new List<int>();
            for (int i = 0; i < numbersCount; i++)
            {
                result.Add(rand.Next(min, max));
            }
            return result;
        }
    }
}
