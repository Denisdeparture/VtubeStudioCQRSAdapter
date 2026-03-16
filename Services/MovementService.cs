using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VtubeStudioAdapter.Commands.PropertyModel.Movement;
using VtubeStudioAdapter.Models;
using VtubeStudioAdapter.Services;
using WebSocketSharp;

namespace VtubeStudioAdapter.Services
{
    public class MovementService
    {
        private readonly WebSocketSessionManager _manager;
        private string? _pluginName;
        private readonly ILogger _logger;
        private Action<VTSData>? _globalAction;

        public MovementService(WebSocketSessionManager manager, ILogger<MovementService> logger)
        {
            _manager = manager;
            _logger = logger;
        }

        public async Task<VTSData> GetArtMeshes(ArtMeshModelQuery query)
        {
            if (string.IsNullOrWhiteSpace(query.PluginName))
            {
                _logger.LogError("PluginName was null or empty in {Query}", nameof(ArtMeshModelQuery));
                return new VTSData();
            }

            var client = _manager.GetInfoConnection(query.PluginName);

            if (client is null)
            {
                _logger.LogError("WebSocket client for plugin {Plugin} was null", query.PluginName);
                return new VTSData();
            }

            client.OnMessage += OnCompleted;

            _logger.LogInformation("Entering {Method}", nameof(GetArtMeshes));
            const string Request = "ArtMeshListRequest";

            _pluginName = query.PluginName;
            var data = (VTSData)query;
            await SendRequest(client, Request, data);
            _globalAction = query.OnCompleted;

            _logger.LogInformation("Exiting {Method}", nameof(GetArtMeshes));
            return data;

        }

        public async Task<VTSData> GetTrackingParametrs(TrackingParametrsQuery query)
        {
            if (string.IsNullOrWhiteSpace(query.PluginName))
            {
                _logger.LogError("PluginName was null or empty in {Query}", nameof(TrackingParametrsQuery));
                return new VTSData();
            }

            var client = _manager.GetInfoConnection(query.PluginName);

            if (client is null)
            {
                _logger.LogError("WebSocket client for plugin {Plugin} was null", query.PluginName);
                return new VTSData();
            }

            client.OnMessage += OnCompleted;

            _logger.LogInformation("Entering {Method}", nameof(GetTrackingParametrs));
            const string Request = "InputParameterListRequest";

            _pluginName = query.PluginName;
            var data = (VTSData)query;
            await SendRequest(client, Request, data);
            _globalAction = query.OnCompleted;

            _logger.LogInformation("Exiting {Method}", nameof(GetTrackingParametrs));
            return data;

        }

        public async Task ChangeValueParametrs(string pluginName, params VTSData.Parametr[] parametr)
        {
            _logger.LogInformation("Entering {Method} with {Count} parameters", nameof(ChangeValueParametrs), parametr?.Length ?? 0);
            const string Request = "InjectParameterDataRequest";

            _pluginName = pluginName;
            var data = new VTSData()
            {
                ParameterValues = parametr
            };
            var model = VtubeStudioModel.CreateModel(ConstStorage.API_NAME, ConstStorage.VERSION, Request, data);
            var json = JsonConvert.SerializeObject(model, ConstStorage.SETTINGS);
            var buffer = Encoding.UTF8.GetBytes(json);

            if (string.IsNullOrWhiteSpace(pluginName))
            {
                _logger.LogError("PluginName was null or empty for ChangeValueParametrs");
                return;
            }

            var client = _manager.GetInfoConnection(pluginName);

            if (client is null)
            {
                _logger.LogError("WebSocket client for plugin {Plugin} was null", pluginName);
                return;
            }

            client.Send(buffer);

            client.OnMessage += OnCompleted;

            _logger.LogInformation("Exiting {Method}", nameof(ChangeValueParametrs));
        }

        private async Task SendRequest(WebSocketSharp.WebSocket client, string messageType, VTSData data)
        {
            _logger.LogInformation("Sending movement request {MessageType}", messageType);
            var model = VtubeStudioModel.CreateModel(ConstStorage.API_NAME, ConstStorage.VERSION, messageType, data);

            var json = JsonConvert.SerializeObject(model, ConstStorage.SETTINGS);

            _logger.LogInformation("Json on movement method {0}", json);

            var buffer = Encoding.UTF8.GetBytes(json);

            client.Send(buffer);
        }

        private async void OnCompleted(object? sender, MessageEventArgs e)
        {
            try
            {
                var json = e.Data;
                _logger.LogDebug("End json {0}", json);
                var obj = JsonConvert.DeserializeObject<VtubeStudioModel>(json);

                if (obj is null || obj.Data is null)
                {
                    _logger.LogError($"[{DateTime.UtcNow}]: data in movement response was null");
                    return;
                }

                if (obj.Data.ErrorID is not null)
                {
                    _logger.LogWarning($"[{DateTime.UtcNow}]: VTube Studio API movement error: {obj.Data.ErrorID} {obj.Data.Message}");
                }

                if (_globalAction is not null)
                {
                    _globalAction?.Invoke(obj.Data);
                }

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
                _logger.LogError(ex, "Error while processing movement response");
            }
        }
    }
}
