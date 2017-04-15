using NetCoreStack.WebSockets;
using NetCoreStack.WebSockets.ProxyClient;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Hisar.Component.Guideline.Core
{
    public class CommandInvocator : IClientWebSocketCommandInvocator
    {
        public async Task InvokeAsync(WebSocketMessageContext context)
        {
            if (context.MessageType == WebSocketMessageType.Text)
            {
                object pageName = null;
                if (context.Header.TryGetValue("pageupdated", out pageName))
                {
                    var pageModel = context.Value;
                }
            }

            await Task.CompletedTask;
        }
    }
}
