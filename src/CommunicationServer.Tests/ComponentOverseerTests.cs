﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Dvrp.Ucc.Commons.Components;
using Dvrp.Ucc.CommunicationServer.Components;
using Dvrp.Ucc.CommunicationServer.Components.Base;
using Xunit;

namespace Dvrp.Ucc.CommunicationServer.Tests
{
    public class ComponentOverseerTests
    {
	    [Fact]
        public void AllTypesOfNodesAreProperlyRegistered()
        {
            var overseer = new ComponentOverseer(5, 5);

            var solvableProblems = new List<string> {"dvrp"};

            var serverInfo = new ServerInfo("127.0.0.1",9135);

            ComponentInfo taskManager = new SolverNodeInfo(1, ComponentType.TaskManager, solvableProblems, 5);
            ComponentInfo computationalNode = new SolverNodeInfo(2, ComponentType.ComputationalNode, solvableProblems, 5);
            ComponentInfo backupServer = new BackupServerInfo(3, serverInfo, 5);

            overseer.TryRegister(taskManager);
            overseer.TryRegister(computationalNode);
            overseer.TryRegister(backupServer);

            Assert.True(overseer.IsRegistered(taskManager.ComponentId));
            Assert.True(overseer.IsRegistered(computationalNode.ComponentId));
            Assert.True(overseer.IsRegistered(backupServer.ComponentId));
        }

        [Fact]
        public void NodeIsDeregisteredAfterTimeout()
        {
            uint communicationTimeout = 1;
            uint checkInterval = 1;
            var stopwatch = new Stopwatch();
            var deregistrationEventLock = new AutoResetEvent(false);

            var overseer = new ComponentOverseer(communicationTimeout, checkInterval);
            overseer.Deregistration += (_, _) => { deregistrationEventLock.Set(); };
            overseer.StartMonitoring();

            var solvableProblems = new List<string> {"dvrp"};

            ComponentInfo computationalNode = new SolverNodeInfo(1, ComponentType.ComputationalNode, solvableProblems, 5);
            overseer.TryRegister(computationalNode);

            stopwatch.Start();

            Assert.True(overseer.IsRegistered(computationalNode.ComponentId));

            deregistrationEventLock.WaitOne(1000*(int) (communicationTimeout + checkInterval + 1));

            stopwatch.Stop();

            Assert.False(overseer.IsRegistered(computationalNode.ComponentId));
            Assert.True((ulong) stopwatch.ElapsedMilliseconds >= communicationTimeout);

            overseer.StopMonitoring();
        }
    }
}