using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VtubeStudioAdapter.Models;

namespace VtubeStudioAdapter.Services
{
    public class StatusService
    {
        private readonly ClientWebSocket _client;
        private readonly CancellationTokenSource _cts;
        private readonly ILogger _logger;

        public StatusService(ClientWebSocket client, CancellationTokenSource cts, ILogger<StatusService> logger)
        {
            _client = client;
            _cts = cts;
            _logger = logger;
        }

        public async Task<VTSData> GetCurrentStatus()
        {
            _logger.LogInformation("Entering {Method}", nameof(GetCurrentStatus));
            const string Request = "APIStateRequest";

            await SendRequest(Request, new VTSData());
            var result = await ConsumeCurrentStatus();
            _logger.LogInformation("Exiting {Method}", nameof(GetCurrentStatus));
            return result;
        }

        private async Task SendRequest(string messageType, VTSData data)
        {
            _logger.LogDebug("Sending status request {MessageType}", messageType);
            var model = VtubeStudioModel.CreateModel(ConstStorage.API_NAME, ConstStorage.VERSION, messageType, data);
            var json = JsonConvert.SerializeObject(model, ConstStorage.SETTINGS);
            var buffer = Encoding.UTF8.GetBytes(json);

            await _client.SendAsync(
                new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text,
                true,
                _cts.Token
            );
        }

        private async Task<VTSData> ConsumeCurrentStatus()
        {
            _logger.LogDebug("Waiting for status response...");
            var buffer = new byte[4096];
            WebSocketReceiveResult? result = null;

            do
            {
                result = await _client.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None
                );

                if (result.MessageType == WebSocketMessageType.Text && result.EndOfMessage)
                {
                    var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var obj = JsonConvert.DeserializeObject<VtubeStudioModel>(json);

                    if (obj is null || obj.Data is null)
                    {
                        _logger.LogError($"[{DateTime.UtcNow}]: data in status response was null");
                        throw new NullReferenceException("Status response data was null");
                    }

                    if (obj.Data.ErrorID is not null)
                    {
                        _logger.LogWarning($"[{DateTime.UtcNow}]: VTube Studio API status error: {obj.Data.ErrorID} {obj.Data.Message}");
                    }

                    _logger.LogDebug("Status response received successfully.");
                    return obj.Data;
                }

            } while (result is not null && !result.CloseStatus.HasValue);

            throw new WebSocketException("WebSocket closed before status response was received.");
        }
    }
}