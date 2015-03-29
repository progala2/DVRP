using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

namespace _15pl04.Ucc.CommunicationServer.Tests
{
    [TestClass]
    public class ComponentMonitorTests
    {
        [TestMethod]
        public void NodesAreProperlyRegistered()
        {
            ulong taskManagerId = ComponentMonitor.Instance.RegisterNode(Commons.ComponentType.TaskManager, 3, new string[]{"dvrp"});
            ulong computationalNodeId = ComponentMonitor.Instance.RegisterNode(Commons.ComponentType.ComputationalNode, 3, new string[] { "dvrp" });

            Assert.IsTrue(ComponentMonitor.Instance.IsRegistered(taskManagerId));
            Assert.IsTrue(ComponentMonitor.Instance.IsRegistered(computationalNodeId));
        }

        [TestMethod]
        public void NodesAreDeregisteredAfterTimeout()
        {
            ComponentMonitor.Instance.StartMonitoring(2000);

            ulong taskManagerId = ComponentMonitor.Instance.RegisterNode(Commons.ComponentType.TaskManager, 3, new string[] { "dvrp" });
            Assert.IsTrue(ComponentMonitor.Instance.IsRegistered(taskManagerId));

            Thread.Sleep(2500);
            Assert.IsFalse(ComponentMonitor.Instance.IsRegistered(taskManagerId));
        }
    }
}
