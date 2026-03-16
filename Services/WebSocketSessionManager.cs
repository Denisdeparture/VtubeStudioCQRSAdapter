using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using VtubeStudioAdapter.Commands;
using VtubeStudioAdapter.Commands.Auth;
using VtubeStudioAdapter.Models;
using WebSocketSharp;

namespace VtubeStudioAdapter.Services
{
    public class WebSocketSessionManager(IMediator mediator, ILogger<WebSocketSessionManager> logger)
    {
        private readonly Dictionary<string, WebSocket> _connections = new();

        public async Task<WebSocket> TryAddConnection(string pluginName, string url)
        {
            if (_connections.TryGetValue(pluginName, out var existingSocket))
            {
                if (existingSocket.IsAlive)
                {
                    RemoveConnection(pluginName);
                }
            }
            WebSocket socket = new WebSocket(url);

            socket.Connect();

            var res = _connections.TryAdd(pluginName, socket);

            if (res is false) throw new Exception("Web socket doesn t add");

            return socket;
        }

        public void RemoveConnection(string pluginName)
        {
            if (_connections.TryGetValue(pluginName, out var webSocket))
            {
                webSocket.Close(CloseStatusCode.Normal);

                _connections.Remove(pluginName);
            }
        }

        public WebSocket? GetInfoConnection(string pluginName)
        {
            if (string.IsNullOrWhiteSpace(pluginName)) throw new NullReferenceException("Plugin name was null");

            _connections.TryGetValue(pluginName, out var webSocket);

            return webSocket;
        }
        public void CleanupStaleConnections()
        {
            foreach (var kvp in _connections)
            {
                if (!kvp.Value.IsAlive)
                {
                    RemoveConnection(kvp.Key);
                }
            }
        }
        public bool IsAuth(string pluginName)
        {
            var socket = GetInfoConnection(pluginName);

            if (socket is null || !socket.IsAlive) return false;

            bool isAuth = false;

            var task = mediator.Send(new StatusVTSModelQuery()
            {
                PluginName = pluginName,
                OnCompleted = (data) =>
                {
                    isAuth = data.CurrentSessionAuthenticated;
                }
            });

            Thread.Sleep(200);

            task.Wait();

            return isAuth;
        }
        public string[] GetAllConnectionsName()
        {
            return _connections.Select(x => x.Key).ToArray();
        }
    }
}