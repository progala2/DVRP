using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using _15pl04.Ucc.Commons.Components;
using _15pl04.Ucc.CommunicationServer.Components;
using _15pl04.Ucc.CommunicationServer.Components.Base;
using Xunit;

namespace _15pl04.Ucc.CommunicationServer.Tests
{
    public class ComponentOverseerTests
    {
        private IComponentOverseer _overseer;

        [Fact]
        public void AllTypesOfNodesAreProperlyRegistered()
        {
            _overseer = new ComponentOverseer(5, 5);

            var solvableProblems = new List<string> {"dvrp"};

            var serverInfo = new ServerInfo
            {
                IpAddress = "127.0.0.1",
                Port = 9135
            };

            ComponentInfo taskManager = new SolverNodeInfo(ComponentType.TaskManager, solvableProblems, 5);
            ComponentInfo computationalNode = new SolverNodeInfo(ComponentType.ComputationalNode, solvableProblems, 5);
            ComponentInfo backupServer = new BackupServerInfo(serverInfo, 5);

            Assert.Null(taskManager.ComponentId);
            Assert.Null(computationalNode.ComponentId);
            Assert.Null(backupServer.ComponentId);

            _overseer.TryRegister(taskManager);
            _overseer.TryRegister(computationalNode);
            _overseer.TryRegister(backupServer);

            Assert.True(_overseer.IsRegistered(taskManager.ComponentId.Value));
            Assert.True(_overseer.IsRegistered(computationalNode.ComponentId.Value));
            Assert.True(_overseer.IsRegistered(backupServer.ComponentId.Value));
        }

        [Fact]
        public void NodeIsDeregisteredAfterTimeout()
        {
            uint communicationTimeout = 1;
            uint checkInterval = 1;
            var stopwatch = new Stopwatch();
            var deregistrationEventLock = new AutoResetEvent(false);

            _overseer = new ComponentOverseer(communicationTimeout, checkInterval);
            _overseer.Deregistration += (o, e) => { deregistrationEventLock.Set(); };
            _overseer.StartMonitoring();

            var solvableProblems = new List<string> {"dvrp"};

            ComponentInfo computationalNode = new SolverNodeInfo(ComponentType.ComputationalNode, solvableProblems, 5);
            _overseer.TryRegister(computationalNode);

            stopwatch.Start();

            Assert.True(_overseer.IsRegistered(computationalNode.ComponentId.Value));

            deregistrationEventLock.WaitOne(1000*(int) (communicationTimeout + checkInterval + 1));

            stopwatch.Stop();

            Assert.False(_overseer.IsRegistered(computationalNode.ComponentId.Value));
            Assert.True((ulong) stopwatch.ElapsedMilliseconds >= communicationTimeout);

            _overseer.StopMonitoring();
        }
    }
}