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
    public class PositionService
    {
        private readonly ClientWebSocket _client;
        private readonly CancellationTokenSource _cts;
        private readonly ILogger _logger;

        public PositionService(ClientWebSocket client, CancellationTokenSource cts, ILogger<PositionService> logger)
        {
            _client = client;
            _cts = cts;
            _logger = logger;
        }

        public async Task<VTSData> ChangePosition(VTSData data)
        {
            _logger.LogInformation("Entering {Method}", nameof(ChangePosition));
            const string Request = "MoveModelRequest";

            await SendRequest(Request, data);

            _logger.LogInformation("Exiting {Method}", nameof(ChangePosition));

            var result = await ConsumeCurrentModelData();

            return result;
        }

        public async Task<VTSData> GetCurrentModelData()
        {
            _logger.LogInformation("Entering {Method}", nameof(GetCurrentModelData));
            const string Request = "CurrentModelRequest";

            await SendRequest(Request, new VTSData());
            var result = await ConsumeCurrentModelData();
            _logger.LogInformation("Exiting {Method}", nameof(GetCurrentModelData));
            return result;
        }

        private async Task SendRequest(string messageType, VTSData data)
        {
            _logger.LogDebug("Sending position request {MessageType}", messageType);
            var model = VtubeStudioModel.CreateModel(ConstStorage.API_NAME, ConstStorage.VERSION, messageType, data);
            var json = JsonConvert.SerializeObject(model, ConstStorage.SETTINGS);


            _logger.LogInformation("Now json on position {0}", json);

            var buffer = Encoding.UTF8.GetBytes(json);

            await _client.SendAsync(
                new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text,
                true,
                _cts.Token
            );
        }

        private async Task<VTSData> ConsumeCurrentModelData()
        {
            _logger.LogDebug("Waiting for current model response...");
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
                        _logger.LogError($"[{DateTime.UtcNow}]: data in current model response was null");
                        throw new NullReferenceException("Current model response data was null");
                    }

                    if (obj.Data.ErrorID is not null)
                    {
                        _logger.LogWarning($"[{DateTime.UtcNow}]: VTube Studio API position error: {obj.Data.ErrorID} {obj.Data.Message}");
                    }

                    return obj.Data;
                }

            } while (result is not null && !result.CloseStatus.HasValue);

            throw new WebSocketException("WebSocket closed before current model response was received.");
        }
    }
}