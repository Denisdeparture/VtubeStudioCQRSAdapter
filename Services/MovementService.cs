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
    public class MovementService
    {
        private readonly ClientWebSocket _client;
        private readonly CancellationTokenSource _cts;
        private readonly ILogger _logger;

        public MovementService(ClientWebSocket client, CancellationTokenSource cts, ILogger<MovementService> logger)
        {
            _client = client;
            _cts = cts;
            _logger = logger;
        }

        public async Task<VTSData> GetArtMeshes()
        {
            _logger.LogInformation("Entering {Method}", nameof(GetArtMeshes));
            const string Request = "ArtMeshListRequest";

            await SendRequest(Request, new VTSData());
            var result = await ReceiveSingleResponse(Request);
            _logger.LogInformation("Exiting {Method}", nameof(GetArtMeshes));
            return result;

        }
        public async Task<VTSData> GetTrackingParametrs()
        {
            _logger.LogInformation("Entering {Method}", nameof(GetTrackingParametrs));
            const string Request = "InputParameterListRequest";

            await SendRequest(Request, new VTSData());
            var result = await ReceiveSingleResponse(Request);
            _logger.LogInformation("Exiting {Method}", nameof(GetTrackingParametrs));
            return result;

        }
        public async Task ChangeValueParametrs(params VTSData.Parametr[] parametr)
        {
            _logger.LogInformation("Entering {Method} with {Count} parameters", nameof(ChangeValueParametrs), parametr?.Length ?? 0);
            const string Request = "InjectParameterDataRequest";

            var data = new VTSData()
            {
                ParameterValues = parametr
            };
            var model = VtubeStudioModel.CreateModel(ConstStorage.API_NAME, ConstStorage.VERSION, Request, data);
            var json = JsonConvert.SerializeObject(model, ConstStorage.SETTINGS);
            var buffer = Encoding.UTF8.GetBytes(json);

            await _client.SendAsync(
                new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text,
                true,
                _cts.Token
            );
            _logger.LogInformation("Exiting {Method}", nameof(ChangeValueParametrs));
        }

        private async Task SendRequest(string messageType, VTSData data)
        {
            _logger.LogInformation("Sending movement request {MessageType}", messageType);
            var model = VtubeStudioModel.CreateModel(ConstStorage.API_NAME, ConstStorage.VERSION, messageType, data);

            var json = JsonConvert.SerializeObject(model, ConstStorage.SETTINGS);

            _logger.LogInformation("Json on movement method {0}", json);

            var buffer = Encoding.UTF8.GetBytes(json);

            await _client.SendAsync(
                new ArraySegment<byte>(buffer),
                WebSocketMessageType.Text,
                true,
                _cts.Token
            );
        }

        private async Task<VTSData> ReceiveSingleResponse(string expectedMessageType)
        {
            _logger.LogInformation("Waiting for movement response for {MessageType}", expectedMessageType);
            var buffer = new byte[512];
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
                    _logger.LogInformation("End json {0}", json);
                    var obj = JsonConvert.DeserializeObject<VtubeStudioModel>(json);

                    if (obj is null || obj.Data is null)
                    {
                        _logger.LogError($"[{DateTime.UtcNow}]: data in movement response was null");
                        throw new NullReferenceException("Movement response data was null");
                    }

                    if (obj.Data.ErrorID is not null)
                    {
                        _logger.LogWarning($"[{DateTime.UtcNow}]: VTube Studio API movement error: {obj.Data.ErrorID} {obj.Data.Message}");
                    }

                    _logger.LogDebug("Movement response received successfully for {MessageType}", expectedMessageType);
                    return obj.Data;
                }

            } while (result is not null && !result.CloseStatus.HasValue);

            throw new WebSocketException("WebSocket closed before movement response was received.");
        }
    }
}
