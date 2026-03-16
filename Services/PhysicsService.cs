using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VtubeStudioAdapter.Commands.PropertyModel.Physics;
using VtubeStudioAdapter.Models;
using VtubeStudioAdapter.Models.PropertyModel.Physics;
using VtubeStudioAdapter.Services;
using WebSocketSharp;

namespace VtubeStudioAdapter.Services
{
    public class PhysicsService
    {
        private readonly WebSocketSessionManager _manager;
        private string? _pluginName;
        private readonly ILogger _logger;
        private Action<VTSData>? _globalAction;

        public PhysicsService(WebSocketSessionManager manager, ILogger<PhysicsService> logger)
        {
            _manager = manager;
            _logger = logger;
        }

        public async Task ChangePhysicParametr(ChangePhysicsParametrs request)
        {
            _logger.LogInformation("Entering {Method} with wind={WindCount}, strength={StrengthCount}", nameof(ChangePhysicParametr), request.Wind?.Length ?? 0, request.Strength?.Length ?? 0);
            const string Request = "SetCurrentModelPhysicsRequest";

            if (string.IsNullOrWhiteSpace(request.PluginName))
            {
                _logger.LogError("PluginName was null or empty in {Request}", nameof(ChangePhysicsParametrs));
                return;
            }

            var client = _manager.GetInfoConnection(request.PluginName);

            if (client is null)
            {
                _logger.LogError("WebSocket client for plugin {Plugin} was null", request.PluginName);
                return;
            }

            var data = (VTSData)request;
            var model = VtubeStudioModel.CreateModel(ConstStorage.API_NAME, ConstStorage.VERSION, Request, data);
            var json = JsonConvert.SerializeObject(model, ConstStorage.SETTINGS);
            var buffer = Encoding.UTF8.GetBytes(json);

            client.Send(buffer);
            _logger.LogInformation("Exiting {Method}", nameof(ChangePhysicParametr));
        }

        public async Task<VTSData> GetPhysicsParametrs(GetPhysicsQuery query)
        {
            if (string.IsNullOrWhiteSpace(query.PluginName))
            {
                _logger.LogError("PluginName was null or empty in {Query}", nameof(GetPhysicsQuery));
                return new VTSData();
            }

            var client = _manager.GetInfoConnection(query.PluginName);

            if (client is null)
            {
                _logger.LogError("WebSocket client for plugin {Plugin} was null", query.PluginName);
                return new VTSData();
            }

            client.OnMessage += OnCompleted;

            _logger.LogInformation("Entering {Method}", nameof(GetPhysicsParametrs));
            const string Request = "GetCurrentModelPhysicsRequest";
            _pluginName = query.PluginName;
            var data = (VTSData)query;
            await SendRequest(client, Request, data);
            _globalAction = query.OnCompleted;

            _logger.LogInformation("Exiting {Method}", nameof(GetPhysicsParametrs));
            return data;
        }

        private async Task SendRequest(WebSocketSharp.WebSocket client, string messageType, VTSData data)
        {
            _logger.LogDebug("Sending physics request {MessageType}", messageType);
            var model = VtubeStudioModel.CreateModel(ConstStorage.API_NAME, ConstStorage.VERSION, messageType, data);
            var json = JsonConvert.SerializeObject(model, ConstStorage.SETTINGS);
            var buffer = Encoding.UTF8.GetBytes(json);

            client.Send(buffer);
        }

        private async void OnCompleted(object? sender, MessageEventArgs e)
        {
            try
            {
                var json = e.Data;
                var obj = JsonConvert.DeserializeObject<VtubeStudioModel>(json);

                if (obj is null || obj.Data is null)
                {
                    _logger.LogError($"[{DateTime.UtcNow}]: data in physics response was null");
                    return;
                }

                if (obj.Data.ErrorID is not null)
                {
                    _logger.LogWarning($"[{DateTime.UtcNow}]: VTube Studio API physics error: {obj.Data.ErrorID} {obj.Data.Message}");
                }

                _globalAction?.Invoke(obj.Data);

                if (!string.IsNullOrWhiteSpace(_pluginName))
                {
                    var client = _manager.GetInfoConnection(_pluginName);
                    client!.OnMessage -= OnCompleted;
                }

                _globalAction = null;
                _pluginName = null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing physics response");
            }
        }
    }
}