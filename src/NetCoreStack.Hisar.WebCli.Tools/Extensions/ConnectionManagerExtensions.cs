using Microsoft.AspNetCore.Routing;
using NetCoreStack.Hisar.WebCli.Tools.Core;
using NetCoreStack.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreStack.Hisar.WebCli.Tools
{
    public static class ConnectionManagerExtensions
    {
        public static async Task BroadcastAsyncFileChanged(this IConnectionManager connectionManager, string content, string fullname)
        {
            if (string.IsNullOrEmpty(HostingHelper.MainAppDirectory))
            {
                // _Layout.cshtml from resources
                fullname = PathUtility.NormalizeToWebPath(fullname);
                await connectionManager.BroadcastBinaryAsync(Encoding.UTF8.GetBytes(content), 
                    new RouteValueDictionary(new { mainappdir = HostingHelper.MainAppDirectory, fileupdated = fullname }));

                return;
            }

            fullname = fullname.Replace(HostingHelper.MainAppDirectory, string.Empty);
            fullname = PathUtility.NormalizeToWebPath(fullname);
            fullname = fullname.Replace("/wwwroot", string.Empty);

            await connectionManager.BroadcastBinaryAsync(Encoding.UTF8.GetBytes(content),
                new RouteValueDictionary(new { mainappdir = HostingHelper.MainAppDirectory, fileupdated = fullname }));
        }
    }
}
