using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Utilities;
using _15pl04.Ucc.MinMaxTaskSolver;
using _15pl04.Ucc.TaskSolver;

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

            var problemType = "UCC.Dvrp";

            string line;
            while ((line = Console.ReadLine()) != "exit")
            {
                // input handling
                if (line == "solve")
                {
                    DvrpProblem problem = new DvrpProblem(12, 100, new[]
                {
                new Depot(0, 0, 0, 640), 
            }, new[]
            {
                new Request(-55, -26, -48, 616, 20),
                new Request(-24, 38, -20, 91, 20),
                new Request(-99, -29, -45, 240, 20),
                new Request(-42, 30, -19, 356, 20),
                new Request(59, 66, -32, 528, 20),
                new Request(55, -35, -42, 459, 20),
                new Request(-42, 3, -19, 433, 20),
                new Request(95, 13, -35, 513, 20),
                new Request(71, -90, -30, 444, 20),
                new Request(38, 32, -26, 44, 20),
                new Request(67, -22, -41, 318, 20),
                new Request(58, -97, -27, 20, 20),
            });
                    var problemData = GenerateProblemData(problem);
                    computationalClient.SendSolveRequest(problemType, problemData, null);
                }
                if (line == "solution")
                {
                    Console.Write("Enter problem id: ");
                    ulong id;
                    if (ulong.TryParse(Console.ReadLine(), out id))
                    {
                        var solutionsMessages = computationalClient.SendSolutionRequest(id);
                        if (solutionsMessages[0].Solutions[0].Type == Commons.Messaging.Models.SolutionsMessage.SolutionType.Final)
                        {
                            DvrpSolution solution;
                            using (var memoryStream = new MemoryStream(solutionsMessages[0].Solutions[0].Data))
                            {
                                var formatter = new BinaryFormatter();
                                solution = (DvrpSolution)formatter.Deserialize(memoryStream);
                            }
                            Console.WriteLine("Result: {0}", solution.FinalDistance);
                        }
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

        private static byte[] GenerateProblemData(DvrpProblem dvrpProblem)
        {
            using (var memoryStream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, dvrpProblem);
                var problemData = memoryStream.ToArray();
                return problemData;
            }
        }
    }
}