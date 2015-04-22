using _15pl04.Ucc.Commons;
using _15pl04.Ucc.Commons.Messaging.Models;
using _15pl04.Ucc.CommunicationServer.Components;
using _15pl04.Ucc.CommunicationServer.Components.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Threading;

namespace _15pl04.Ucc.CommunicationServer.Tests
{
    [TestClass]
    public class ComponentMonitorTests
    {
        private IComponentOverseer _overseer;

        [TestMethod]
        public void AllTypesOfNodesAreProperlyRegistered()
        {
            _overseer = new ComponentOverseer(10000, 5000);

            var solvableProblems = new List<string>();
            solvableProblems.Add("dvrp");

            var serverInfo = new ServerInfo();
            serverInfo.IpAddress = "127.0.0.1";
            serverInfo.Port = 9135;

            ComponentInfo taskManager = new SolverNodeInfo(ComponentType.TaskManager, solvableProblems, 5);
            ComponentInfo computationalNode = new SolverNodeInfo(ComponentType.ComputationalNode, solvableProblems, 5);
            ComponentInfo backupServer = new BackupServerInfo(serverInfo, 5);

            Assert.IsNull(taskManager.ComponentId);
            Assert.IsNull(computationalNode.ComponentId);
            Assert.IsNull(backupServer.ComponentId);

            _overseer.TryRegister(taskManager);
            _overseer.TryRegister(computationalNode);
            _overseer.TryRegister(backupServer);

            Assert.IsTrue(_overseer.IsRegistered(taskManager.ComponentId.Value));
            Assert.IsTrue(_overseer.IsRegistered(computationalNode.ComponentId.Value));
            Assert.IsTrue(_overseer.IsRegistered(backupServer.ComponentId.Value));
        }

        [TestMethod]
        public void NodeIsDeregisteredAfterTimeout()
        {
            ulong communicationTimeout = 500;
            int checkInterval = 100;

            _overseer = new ComponentOverseer(communicationTimeout, checkInterval);
            _overseer.StartMonitoring();

            var solvableProblems = new List<string>();
            solvableProblems.Add("dvrp");

            ComponentInfo computationalNode = new SolverNodeInfo(ComponentType.ComputationalNode, solvableProblems, 5);

            _overseer.TryRegister(computationalNode);
            Assert.IsTrue(_overseer.IsRegistered(computationalNode.ComponentId.Value));

            Thread.Sleep(TimeSpan.FromMilliseconds(communicationTimeout + 100));
            Assert.IsFalse(_overseer.IsRegistered(computationalNode.ComponentId.Value));

            _overseer.StopMonitoring();
        }

        // TODO more tests
    }
}
