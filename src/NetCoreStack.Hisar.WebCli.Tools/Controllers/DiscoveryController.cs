using NetCoreStack.Hisar.WebCli.Tools.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using NetCoreStack.WebSockets;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NetCoreStack.Hisar.WebCli.Tools.Core;
using System.Collections.Generic;

namespace NetCoreStack.Hisar.WebCli.Tools.Controllers
{
    [Route("api/[controller]")]
    public class DiscoveryController : Controller
    {
        private readonly IConnectionManager _connectionManager;
        private readonly IMemoryCache _memoryCache;

        protected IServiceProvider Resolver
        {
            get
            {
                return HttpContext?.RequestServices;
            }
        }

        public DiscoveryController(IConnectionManager connectionManager,
            IMemoryCache memoryCache)
        {
            _connectionManager = connectionManager;
            _memoryCache = memoryCache;
        }

        [HttpGet(nameof(Environments))]
        public IActionResult Environments()
        {
            var envContext = Resolver.GetService<EnvironmentContext>();
            IDictionary<string, object> values = envContext.ToJson();
            values.Add(nameof(Environment.ProcessorCount), Environment.ProcessorCount);
            return Json(values);
        }

        [HttpPost(nameof(SendAsync))]
        public async Task<IActionResult> SendAsync([FromBody]SimpleCacheItem model)
        {
            var echo = $"Echo from server '{model.Id}' '{model.Name}' - {DateTime.Now}";
            var obj = new { message = echo };
            var webSocketContext = new WebSocketMessageContext { Command = WebSocketCommands.DataSend, Value = obj };
            await _connectionManager.BroadcastAsync(webSocketContext);
            return Ok();
        }

        [HttpGet(nameof(GetConnections))]
        public IActionResult GetConnections()
        {
            var connections = _connectionManager.Connections
                .Select(x => new { ConnectionId = x.Value.ConnectionId, ConnectorName = x.Value.ConnectorName, State = x.Value.WebSocket.State });

            return Json(connections);
        }
    }
}
