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
    public class PhysicsService
    {
        private readonly ClientWebSocket _client;
        private readonly CancellationTokenSource _cts;
        private readonly ILogger _logger;

        public PhysicsService(ClientWebSocket client, CancellationTokenSource cts, ILogger<PhysicsService> logger)
        {
            _client = client;
            _cts = cts;
            _logger = logger;
        }

        public async Task ChangePhysicParametr(
        VTSData.PhysicParamter[] windParamters,
        VTSData.PhysicParamter[] strengthParamters)
        {
            _logger.LogInformation("Entering {Method} with wind={WindCount}, strength={StrengthCount}", nameof(ChangePhysicParametr), windParamters?.Length ?? 0, strengthParamters?.Length ?? 0);
            const string Request = "SetCurrentModelPhysicsRequest";

            var data = new VTSData()
            {
                Wind = windParamters,
                Strength = strengthParamters
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
            _logger.LogInformation("Exiting {Method}", nameof(ChangePhysicParametr));
        }

        public async Task<VTSData> GetPhysicsParametrs()
        {
            _logger.LogInformation("Entering {Method}", nameof(GetPhysicsParametrs));
            const string Request = "GetCurrentModelPhysicsRequest";
            await SendRequest(Request, new VTSData());
            var result = await ConsumePhysicsParametr();
            _logger.LogInformation("Exiting {Method}", nameof(GetPhysicsParametrs));
            return result;
        }

        private async Task SendRequest(string messageType, VTSData data)
        {
            _logger.LogDebug("Sending physics request {MessageType}", messageType);
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

        private async Task<VTSData> ConsumePhysicsParametr()
        {
            _logger.LogDebug("Waiting for physics response...");
            var buffer = new byte[8192];
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
                        _logger.LogError($"[{DateTime.UtcNow}]: data in physics response was null");
                        throw new NullReferenceException("Physics response data was null");
                    }

                    if (obj.Data.ErrorID is not null)
                    {
                        _logger.LogWarning($"[{DateTime.UtcNow}]: VTube Studio API physics error: {obj.Data.ErrorID} {obj.Data.Message}");
                    }

                    return obj.Data;
                }

            } while (result is not null && !result.CloseStatus.HasValue);

            throw new WebSocketException("WebSocket closed before physics response was received.");
        }
    }
}