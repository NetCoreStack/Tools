using System;
using System.Net;
using System.Net.Sockets;

namespace NetCoreStack.Hisar.WebCli.Tools.Core
{
    public static class HostingHelper
    {
        public static string MainAppDirectory { get; set; }

        private static int HostingPort(int defaultPort = 0)
        {
            var listener = new TcpListener(IPAddress.Any, defaultPort);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        public static int GetHostingPort()
        {
            var port = 0;
            try
            {
                port = HostingPort(1444);
            }
            catch (Exception ex)
            {
                // noop
            }
            finally
            {
                port = HostingPort();
            }

            return port;
        }
    }
}
