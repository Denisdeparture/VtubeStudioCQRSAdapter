using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VtubeStudioAdapter.Commands.Position;
using VtubeStudioAdapter.Сommands.Position;
using VtubeStudioAdapter.Models;
using VtubeStudioAdapter.Services;
using WebSocketSharp;

namespace VtubeStudioAdapter.Services
{
    public class PositionService
    {
        private readonly WebSocketSessionManager _manager;
        private string? _pluginName;
        private readonly ILogger _logger;
        private Action<VTSData>? _globalAction;

        public PositionService(WebSocketSessionManager manager, ILogger<PositionService> logger)
        {
            _manager = manager;
            _logger = logger;
        }

        public async Task<VTSData> ChangePosition(ChangeModelPositionCommand data)
        {
            if (string.IsNullOrWhiteSpace(data.PluginName))
            {
                _logger.LogError("PluginName was null or empty in {Command}", nameof(ChangeModelPositionCommand));
                return new VTSData();
            }

            var client = _manager.GetInfoConnection(data.PluginName);

            if (client is null)
            {
                _logger.LogError("WebSocket client for plugin {Plugin} was null", data.PluginName);
                return new VTSData();
            }

            _logger.LogInformation("Entering {Method}", nameof(ChangePosition));
            const string Request = "MoveModelRequest";

            var vtsData = (VTSData)data;
            await SendRequest(client, Request, vtsData);

            _logger.LogInformation("Exiting {Method}", nameof(ChangePosition));

            _logger.LogInformation("Exiting {Method}", nameof(ChangePosition));
            return new VTSData();
        }

        public async Task<VTSData> GetCurrentModelData(GetCurrentModelQuery query)
        {
            if (string.IsNullOrWhiteSpace(query.PluginName))
            {
                _logger.LogError("PluginName was null or empty in {Query}", nameof(GetCurrentModelQuery));
                return new VTSData();
            }

            var client = _manager.GetInfoConnection(query.PluginName);

            if (client is null)
            {
                _logger.LogError("WebSocket client for plugin {Plugin} was null", query.PluginName);
                return new VTSData();
            }

            client.OnMessage += OnCompleted;

            _logger.LogInformation("Entering {Method}", nameof(GetCurrentModelData));
            const string Request = "CurrentModelRequest";

            _pluginName = query.PluginName;
            var data = (VTSData)query;
            await SendRequest(client, Request, data);
            _globalAction = query.OnCompleted;

            _logger.LogInformation("Exiting {Method}", nameof(GetCurrentModelData));
            return data;
        }

        private async Task SendRequest(WebSocketSharp.WebSocket client, string messageType, VTSData data)
        {

            _logger.LogDebug("Sending position request {MessageType}", messageType);
            var model = VtubeStudioModel.CreateModel(ConstStorage.API_NAME, ConstStorage.VERSION, messageType, data);
            var json = JsonConvert.SerializeObject(model, ConstStorage.SETTINGS);


            _logger.LogInformation("Now json on position {0}", json);

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
                    _logger.LogError($"[{DateTime.UtcNow}]: data in current model response was null");
                    return;
                }

                if (obj.Data.ErrorID is not null)
                {
                    _logger.LogWarning($"[{DateTime.UtcNow}]: VTube Studio API position error: {obj.Data.ErrorID} {obj.Data.Message}");
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
                _logger.LogError(ex, "Error while processing current model response");
            }
        }
    }
}