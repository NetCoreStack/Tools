using NetCoreStack.Hisar.WebCli.Tools.Models;
using NetCoreStack.WebSockets;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NetCoreStack.Hisar.WebCli.Tools.Context;
using System.IO;

namespace NetCoreStack.Hisar.WebCli.Tools.Core
{
    public class ServerWebSocketCommandInvocator : IServerWebSocketCommandInvocator
    {
        private readonly IServiceProvider _applicationServices;
        private readonly IConnectionManager _connectionManager;
        public ServerWebSocketCommandInvocator(IConnectionManager connectionManager, IServiceProvider applicationServices)
        {
            _applicationServices = applicationServices;
            _connectionManager = connectionManager;
        }

        public async Task InvokeAsync(WebSocketMessageContext context)
        {
            if (context.MessageType == WebSocketMessageType.Text
                && context.Command == WebSocketCommands.DataSend)
            {
                if (context.Header != null)
                {
                    if (context.Header.TryGetValue("layoutrequest", out object layoutrequest))
                    {
                        if (!string.IsNullOrEmpty(HostingHelper.MainAppDirectory))
                        {
                            var layoutPagePath = PathUtility.GetLayoutPagePath(HostingHelper.MainAppDirectory);
                            var pageContent = File.ReadAllText(layoutPagePath);
                            await _connectionManager.BroadcastAsyncFileChanged(pageContent, layoutPagePath);
                            return;
                        }

                        using (var scope = _applicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                        using (var db = scope.ServiceProvider.GetService<HisarCliContext>())
                        {
                            var page = db.Set<Page>().FirstOrDefault(x => x.PageType == PageType.Layout);
                            await _connectionManager.BroadcastAsyncFileChanged(page.Content, page.Name);
                        }
                    }
                }
            }
        }
    }
}
