using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;
using System;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class MarshallerTests
    {
        [TestMethod]
        public void TestUnmarshall()
        {
            using (var noOperationStreamReader = new StreamReader("XmlMessages/NoOperation.xml", Encoding.UTF8))
            using (var statusStreamReader = new StreamReader("XmlMessages/Status.xml", Encoding.UTF8))
            {
                var noOperationMessageContent = noOperationStreamReader.ReadToEnd();
                var noOperationMessageBytes = Encoding.UTF8.GetBytes(noOperationMessageContent);
                var statusMessageContent = statusStreamReader.ReadToEnd();
                var statusMessageBytes = Encoding.UTF8.GetBytes(statusMessageContent);

                var buffer = new byte[noOperationMessageBytes.Length + 1 + statusMessageBytes.Length];
                Buffer.BlockCopy(noOperationMessageBytes, 0, buffer, 0, noOperationMessageBytes.Length);
                buffer[noOperationMessageBytes.Length] = 23;
                Buffer.BlockCopy(statusMessageBytes, 0, buffer, noOperationMessageBytes.Length + 1, statusMessageBytes.Length);

                var messages = (new Marshaller()).Unmarshall(buffer);
                Assert.IsTrue(messages.Length == 2);
            }
        }

        [TestMethod]
        public void TestMarshall()
        {
            Message[] tstClass =
            {
                new SolutionRequestMessage { ProblemInstanceId = 2 }, 
                new NoOperationMessage
                {
                    BackupServers = new List<ServerInfo>
                    {
                        new ServerInfo
                        {
                            IpAddress = "ff", Port = 999
                        }
                    }
                },
                new DivideProblemMessage()
                {
                    ComputationalNodes = 10, 
                    ProblemData = new byte[] {1},
                    ProblemInstanceId = 2,
                    NodeId = 3,
                    ProblemType = "ss"
                }
            };
            var data = (new Marshaller()).Marshall(tstClass);
            Assert.IsTrue(data.Count(i => i == 23) == 2);
            var str = Encoding.UTF8.GetString(data);
            Assert.IsTrue(str.Contains("SolutionRequest"));
            Assert.IsTrue(str.Contains("NoOperation"));
            Assert.IsTrue(str.Contains("DivideProblem"));
        }

        [TestMethod]
        public void TestMarshallAndUnMarshall()
        {
            Message[] tstClass =
            {
                new SolutionRequestMessage { ProblemInstanceId = 2 }, 
                new NoOperationMessage
                {
                    BackupServers = new List<ServerInfo>
                    {
                        new ServerInfo
                        {
                            IpAddress = "ff"
                        }
                    }
                },
                new DivideProblemMessage()
                {
                    ProblemData = new byte[] {1},
                    ProblemType = "ss"
                },
                new PartialProblemsMessage()
                {
                    CommonData = new byte[] {1},
                    PartialProblems = new List<PartialProblem>()
                    {
                        new PartialProblem()
                        {
                            Data = new byte[] {1},
                        }
                    },
                    ProblemType = "ss",
                },
                new ErrorMessage()
                {
                    ErrorText = "",
                    ErrorType = ErrorType.ExceptionOccured
                },
                new RegisterMessage()
                {
                    ComponentType = ComponentType.CommunicationServer,
                    SolvableProblems = new List<string>()
                    {
                        "s"
                    }
                },
                new RegisterResponseMessage(),
                new SolutionRequestMessage()
                {
                    ProblemInstanceId = 5
                },
                new SolutionsMessage()
                {
                    ProblemType = "s",
                    Solutions = new List<Solution>()
                    {
                        new Solution()
                        {
                            Type = Solution.SolutionType.Final,
                            Data = new byte[] {3},
                        }
                    }
                },
                new SolveRequestMessage()
                {
                    ProblemType = "ss",
                    ProblemData = new byte[] {1}
                },
                new SolveRequestResponseMessage(),
                new StatusMessage()
                {
                    Threads = new List<ThreadStatus>()
                    {
                        new ThreadStatus()
                        {
                            ProblemType = "ss",
                            State = ThreadStatus.ThreadState.Busy
                        }
                    }
                }
            };
            var data = (new Marshaller()).Marshall(tstClass);
            (new Marshaller()).Unmarshall(data);
        }
    }
}
