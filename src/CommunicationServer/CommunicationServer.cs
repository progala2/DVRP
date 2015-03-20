using _15pl04.Ucc.CommunicationServer.Collections;
using System;

namespace _15pl04.Ucc.CommunicationServer
{
    public class CommunicationServer
    {
        private AsyncTcpServer _tcpServer;
        private ComponentStateMonitor _componentStateMonitor;

        static void Main(string[] args)
        {
            var config = new ServerConfig(args);

            var inputQueue = new InputMessageQueue();
            var outputQueue = new OutputMessageQueue();

            // start server with given correct options or exit

            /*
             * keyboard input handling;
             * invoking CommuncationServer methods (stop or whatever)
             */

            /*
             * cleanup
             */
        }
    }
}
