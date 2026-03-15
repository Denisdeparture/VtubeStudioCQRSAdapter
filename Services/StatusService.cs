using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VtubeStudioAdapter.Commands;
using VtubeStudioAdapter.Models;
using WebSocketSharp;
using WebSocket = WebSocketSharp.WebSocket;

namespace VtubeStudioAdapter.Services
{
    public class StatusService
    {
        private readonly WebSocket _client;
        private readonly ILogger _logger;
        private Action<VTSData>? _globalAction;

        public StatusService(WebSocket client, ILogger<StatusService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<VTSData> GetCurrentStatus(StatusVTSModelQuery query)
        {
            _client.OnMessage += OnCompleted;

            _logger.LogInformation("Entering {Method}", nameof(GetCurrentStatus));
            const string Request = "APIStateRequest";

            await SendRequest(Request, new VTSData());
            _globalAction = query.OnCompleted;

            _logger.LogInformation("Exiting {Method}", nameof(GetCurrentStatus));
            return new VTSData();
        }

        private async Task SendRequest(string messageType, VTSData data)
        {
            _logger.LogDebug("Sending status request {MessageType}", messageType);
            var model = VtubeStudioModel.CreateModel(ConstStorage.API_NAME, ConstStorage.VERSION, messageType, data);
            var json = JsonConvert.SerializeObject(model, ConstStorage.SETTINGS);
            var buffer = Encoding.UTF8.GetBytes(json);

            _client.Send(buffer);
        }

        private async void OnCompleted(object? sender, MessageEventArgs e)
        {
            try
            {
                var json = e.Data;
                var obj = JsonConvert.DeserializeObject<VtubeStudioModel>(json);

                if (obj is null || obj.Data is null)
                {
                    _logger.LogError($"[{DateTime.UtcNow}]: data in status response was null");
                    return;
                }

                if (obj.Data.ErrorID is not null)
                {
                    _logger.LogWarning($"[{DateTime.UtcNow}]: VTube Studio API status error: {obj.Data.ErrorID} {obj.Data.Message}");
                }

                _logger.LogDebug("Status response received successfully.");
                _globalAction(obj.Data);
                _client.OnMessage -= OnCompleted;
                _globalAction = null;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing status response");
            }
        }
    }
}