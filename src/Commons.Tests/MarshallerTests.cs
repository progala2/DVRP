using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _15pl04.Ucc.Commons.Messaging;
using _15pl04.Ucc.Commons.Messaging.Models;

namespace _15pl04.Ucc.Commons.Tests
{
    [TestClass]
    public class MarshallerTests
    {
        [TestMethod]
        public void TestUnmarshall()
        {
            using (var reader = new FileStream("XmlMessages/NoOperation.xml", FileMode.Open))
            {
                var buffer = new byte[(reader.Length - 2)*2];
                reader.Position = 3;
                reader.Read(buffer, 0, (int) reader.Length - 3);
                reader.Position = 3;
                buffer[buffer.Length - 1] = buffer[reader.Length - 3] = 23;
                reader.Read(buffer, (int)reader.Length - 2, (int)reader.Length - 3);
                Assert.IsTrue((new Marshaller()).Unmarshall(buffer).Length == 2);
            }
        }

        [TestMethod]
        public void TestMarshall()
        {
            Message[] tstClass =
            {
                new SolutionRequestMessage { Id = 2 }, 
                new NoOperationMessage
                {
                    BackupCommunicationServers = new List<BackupCommunicationServer>
                    {
                        new BackupCommunicationServer
                        {
                            Address = "ff", Port = 999
                        }
                    }
                },
                new DivideProblemMessage()
                {
                    ComputationalNodes = 10, 
                    Data = new byte[] {1},
                    Id = 2,
                    NodeId = 3,
                    ProblemType = "ss"
                }
            };
            var data = (new Marshaller()).Marshall(tstClass);
            Assert.IsTrue(data.Count(i => i == 23) == 3);
            var str = Encoding.ASCII.GetString(data);
            Assert.IsTrue(str.Contains("SolutionRequest"));
            Assert.IsTrue(str.Contains("NoOperation"));
            Assert.IsTrue(str.Contains("DivideProblem"));
        }

        [TestMethod]
        public void TestMarshallAndUnMarshall()
        {
            Message[] tstClass =
            {
                new SolutionRequestMessage { Id = 2 }, 
                new NoOperationMessage
                {
                    BackupCommunicationServers = new List<BackupCommunicationServer>
                    {
                        new BackupCommunicationServer
                        {
                            Address = "ff"
                        }
                    }
                },
                new DivideProblemMessage()
                {
                    Data = new byte[] {1},
                    ProblemType = "ss"
                },
                new PartialProblemsMessage()
                {
                    CommonData = new byte[] {1},
                    PartialProblems = new List<RegisterResponsePartialProblem>()
                    {
                        new RegisterResponsePartialProblem()
                        {
                            Data = new byte[] {1},
                        }
                    },
                    ProblemType = "ss",
                },
                new ErrorMessage()
                {
                    ErrorMessageText = "",
                    ErrorMessageType = ErrorMessageErrorType.ExceptionOccured
                },
                new RegisterMessage()
                {
                    Type = ComponentType.CommunicationServer,
                    SolvableProblems = new List<string>()
                    {
                        "s"
                    }
                },
                new RegisterResponseMessage(),
                new SolutionRequestMessage()
                {
                    Id = 5
                },
                new SolutionsMessage()
                {
                    ProblemType = "s",
                    Solutions = new List<SolutionsSolution>()
                    {
                        new SolutionsSolution()
                        {
                            Type = SolutionType.Final,
                            Data = new byte[] {3},
                        }
                    }
                },
                new SolveRequestMessage()
                {
                    ProblemType = "ss",
                    Data = new byte[] {1}
                },
                new SolveRequestResponseMessage(),
                new StatusMessage()
                {
                    Threads = new List<StatusThread>()
                    {
                        new StatusThread()
                        {
                            ProblemType = "ss",
                            State = ThreadState.Busy
                        }
                    }
                }
            };
            var data = (new Marshaller()).Marshall(tstClass);
            (new Marshaller()).Unmarshall(data);
        }
    }
}
