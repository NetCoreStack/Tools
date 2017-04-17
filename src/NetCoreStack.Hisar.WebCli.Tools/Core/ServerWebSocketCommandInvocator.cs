using Microsoft.AspNetCore.Routing;
using NetCoreStack.Hisar.WebCli.Tools.Models;
using NetCoreStack.WebSockets;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NetCoreStack.Hisar.WebCli.Tools.Context;

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
                        using (var scope = _applicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                        using (var db = scope.ServiceProvider.GetService<HisarCliContext>())
                        {
                            var page = db.Set<Page>().FirstOrDefault(x => x.PageType == PageType.Layout);
                            await _connectionManager.BroadcastBinaryAsync(Encoding.UTF8.GetBytes(page.Content),
                                new RouteValueDictionary(new { pageupdated = page.Name }));
                        }
                    }
                }
            }
        }
    }
}
