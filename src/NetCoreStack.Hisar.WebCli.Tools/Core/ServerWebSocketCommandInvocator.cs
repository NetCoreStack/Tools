using NetCoreStack.WebSockets;
using System.Threading.Tasks;

namespace NetCoreStack.Hisar.WebCli.Tools.Core
{
    public class ServerWebSocketCommandInvocator : IServerWebSocketCommandInvocator
    {
        public async Task InvokeAsync(WebSocketMessageContext context)
        {
            await Task.CompletedTask;
        }
    }
}
