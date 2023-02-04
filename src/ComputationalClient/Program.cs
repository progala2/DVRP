using System;
using System.IO;
using System.Text;
using System.Text.Json;
using Dvrp.Ucc.Commons.Config;
using Dvrp.Ucc.Commons.Exceptions;
using Dvrp.Ucc.Commons.Logging;
using Dvrp.Ucc.Commons.Messaging;
using Dvrp.Ucc.Commons.Messaging.Models;
using Dvrp.Ucc.Commons.Utilities;

namespace Dvrp.Ucc.ComputationalClient
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        private static readonly ILogger Logger = new ConsoleLogger();

        private static void Main(string[] args)
        {
            ComputationalClient computationalClient;
            try
            {
                var config = ComponentConfigurationSection.LoadConfig("componentConfig", args);

                var serverAddress = IpEndPointParser.Parse(config.PrimaryServer.Address, config.PrimaryServer.Port);
                //string taskSolversDirectoryRelativePath = config.TaskSolversPath;

                Logger.Info("Server address: " + serverAddress);

                computationalClient = new ComputationalClient(serverAddress);
            }
            catch (Exception ex)
            {
                var errorText = $"{ex.GetType().FullName}:{ex.Message}";
                if (ex.InnerException != null)
                    errorText += $"|({ex.InnerException.GetType().FullName}:{ex.InnerException.Message})";
                Logger.Error(errorText);
                return;
            }

            computationalClient.MessageSent += computationalClient_MessageSent;
            computationalClient.MessageReceived += computationalClient_MessageReceived;
            computationalClient.MessageSendingException += computationalClient_MessageSendingException;
            computationalClient.MessageHandlingException += computationalClient_MessageHandlingException;

            while (true)
            {
                Console.WriteLine(@"commands: -stop, quit, exit -solve -solution");

                var line = Console.ReadLine();
                if (line != null) line = line.ToLower();
                else continue;

                switch (line)
                {
                    case "stop":
                    case "quit":
                    case "exit":
                        return;
                    case "solve":
                        Console.WriteLine(@"File name that contains the problem: ");
                        line = Console.ReadLine();
                        if (line == null)
                            goto case "solve";
                        byte[] problemData;
                        try
                        {
	                        using var memoryStream = new MemoryStream();
	                        JsonSerializer.Serialize(memoryStream, File.ReadAllText(line, Encoding.UTF8));
	                        problemData = memoryStream.ToArray();
                        }
                        catch (Exception)
                        {
                            Console.WriteLine(@"Wrong file!");
                            continue;
                        }
                        Console.WriteLine(@"Timeout in seconds: ");
                        if (!ulong.TryParse(Console.ReadLine(), out var timeout))
                        {
                            timeout = (ulong)TimeSpan.MaxValue.TotalSeconds;
                        }
                        Console.WriteLine(@"Problem type (for example 'dvrp'): ");
                        line = Console.ReadLine();
                        if (line == null)
                            goto case "solve";
                        line = line.ToLower();
                        if (line == "dvrp")
                        {
                            var problemType = "UCC.Dvrp";
                            computationalClient.SendSolveRequest(problemType, problemData, timeout);
                        }
                        break;
                    case "solution":
                        Console.Write(@"Enter problem id: ");
                        if (ulong.TryParse(Console.ReadLine(), out var id))
                        {
                            var solutionsMessages = computationalClient.SendSolutionRequest(id);
                            if (solutionsMessages is { Count: > 0 })
                            {
                                switch (solutionsMessages[0].Solutions[0].Type)
                                {
                                    case SolutionsMessage.SolutionType.Ongoing:
                                        Console.WriteLine(@"Server is still solving the problem");
                                        break;
                                    case SolutionsMessage.SolutionType.Partial:
                                    case SolutionsMessage.SolutionType.Final:
                                        string problem;
                                        using (var mem = new MemoryStream(solutionsMessages[0].Solutions[0].Data))
                                        {
                                            problem = JsonSerializer.Deserialize<string>(mem) ?? throw new ParsingNullException(nameof(solutionsMessages));
                                        }
                                        Console.WriteLine(@"Result message: {0}", problem);
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                        }
                        else
                            Console.WriteLine(@"Parsing error!");
                        break;
                }
            }
        }

        private static void computationalClient_MessageSent(object? sender, MessageEventArgs e)
        {
            Logger.Info(e.Message.ToString());
        }

        private static void computationalClient_MessageReceived(object? sender, MessageEventArgs e)
        {
            Logger.Info(e.Message.ToString());
        }

        private static void computationalClient_MessageSendingException(object? sender, MessageExceptionEventArgs e)
        {
            Logger.Warn(e.Message + "\n" + e.Exception);
        }

        static void computationalClient_MessageHandlingException(object? sender, MessageExceptionEventArgs e)
        {
            Logger.Warn(e.Message + "\n" + e.Exception);
        }
    }
}