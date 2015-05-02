using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Utilities;
using _15pl04.Ucc.MinMaxTaskSolver;

namespace _15pl04.Ucc.ComputationalClient
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var appSettings = ConfigurationManager.AppSettings;
            var primaryCSaddress = appSettings["primaryCSaddress"];
            var primaryCSport = appSettings["primaryCSport"];
            var serverAddress = IpEndPointParser.Parse(primaryCSaddress, primaryCSport);
            Console.WriteLine("server address from App.config: " + serverAddress);

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
                    var minMaxProblem = new MmProblem(numbers);
                    var problemData = GenerateProblemData(minMaxProblem);
                    computationalClient.SendSolveRequest(problemType, problemData, null);
                }
                if (line == "solution")
                {
                    Console.Write("Enter problem id: ");
                    uint id;
                    uint.TryParse(Console.ReadLine(), out id);
                    var solutionsMessages = computationalClient.SendSolutionRequest(id);
                }
            }
        }

        private static void computationalClient_MessageSent(object sender, MessageEventArgs e)
        {
            ColorfulConsole.WriteMessageInfo("Sent", e.Message);
        }

        private static void computationalClient_MessageReceived(object sender, MessageEventArgs e)
        {
            ColorfulConsole.WriteMessageInfo("Received", e.Message);
        }

        private static void computationalClient_MessageSendingException(object sender, MessageExceptionEventArgs e)
        {
            ColorfulConsole.WriteMessageExceptionInfo("Message sending exception", e.Message, e.Exception);
        }

        private static byte[] GenerateProblemData(MmProblem minMaxProblem)
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
            var result = new List<int>(numbersCount);
            for (var i = 0; i < numbersCount; i++)
            {
                result.Add(rand.Next(min, max));
            }
            return result;
        }
    }
}