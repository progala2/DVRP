using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using _15pl04.Ucc.Commons.Config;
using _15pl04.Ucc.Commons.Logging;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.Commons.Utilities;

namespace _15pl04.Ucc.ComputationalClient
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

            string line;
            while (true)
            {
                Console.WriteLine(@"commands: -stop, quit, exit -solve -solution");

                line = Console.ReadLine();
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
                            using (var memoryStream = new MemoryStream())
                            {
                                var formatter = new BinaryFormatter();
                                formatter.Serialize(memoryStream, File.ReadAllText(line, Encoding.UTF8));
                                problemData = memoryStream.ToArray();
                            }
                        }
                        catch (Exception)
                        {
                            Console.WriteLine(@"Wrong file!");
                            continue;
                        }
                        Console.WriteLine(@"Timeout in seconds: ");
                        ulong timeout;
                        if (!ulong.TryParse(Console.ReadLine(), out timeout))
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
                        ulong id;
                        if (ulong.TryParse(Console.ReadLine(), out id))
                        {
                            var solutionsMessages = computationalClient.SendSolutionRequest(id);
                            if (solutionsMessages != null && solutionsMessages.Count > 0)
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
                                            var formatter = new BinaryFormatter();
                                            problem = (string)formatter.Deserialize(mem);
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

        private static void computationalClient_MessageSent(object sender, MessageEventArgs e)
        {
            Logger.Info(e.Message.ToString());
        }

        private static void computationalClient_MessageReceived(object sender, MessageEventArgs e)
        {
            Logger.Info(e.Message.ToString());
        }

        private static void computationalClient_MessageSendingException(object sender, MessageExceptionEventArgs e)
        {
            Logger.Warn(e.Message + "\n" + e.Exception);
        }

        static void computationalClient_MessageHandlingException(object sender, MessageExceptionEventArgs e)
        {
            Logger.Warn(e.Message + "\n" + e.Exception);
        }
    }
}