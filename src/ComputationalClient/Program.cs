using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Config;
using _15pl04.Ucc.Commons.Logging;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Utilities;
using _15pl04.Ucc.MinMaxTaskSolver;

namespace _15pl04.Ucc.ComputationalClient
{
    public class Program
    {
        private static ILogger _logger = new TraceSourceLogger("ComputationalClient");

        private static void Main(string[] args)
        {
            ComputationalClient computationalClient;
            try
            {
                ComponentConfigurationSection config = ComponentConfigurationSection.LoadConfig("componentConfig", args);

                IPEndPoint serverAddress = IpEndPointParser.Parse(config.PrimaryServer.Address, config.PrimaryServer.Port);
                //string taskSolversDirectoryRelativePath = config.TaskSolversPath;

                _logger.Info("Server address: " + serverAddress);

                computationalClient = new ComputationalClient(serverAddress);
            }
            catch (Exception ex)
            {
                var errorText = string.Format("{0}:{1}", ex.GetType().FullName, ex.Message);
                if (ex.InnerException != null)
                    errorText += string.Format("|({0}:{1})", ex.InnerException.GetType().FullName, ex.InnerException.Message);
                _logger.Error(errorText);
                return;
            }

            computationalClient.MessageSendingException += computationalClient_MessageSendingException;
            computationalClient.MessageReceived += computationalClient_MessageReceived;
            computationalClient.MessageSent += computationalClient_MessageSent;

            var problemType = "_15pl04.UCC.MinMax";

            string line;
            while (true)
            {
                line = Console.ReadLine().ToLower();
                if (line == "stop" || line == "quit" || line == "exit")
                    break;

                // TODO
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
                    ulong id;
                    if (ulong.TryParse(Console.ReadLine(), out id))
                    {
                        var solutionsMessages = computationalClient.SendSolutionRequest(id);
                        //
                    }
                    else
                        Console.WriteLine("Parsing error!");
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