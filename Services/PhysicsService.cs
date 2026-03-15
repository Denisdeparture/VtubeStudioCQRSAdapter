using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VtubeStudioAdapter.Commands.PropertyModel.Physics;
using VtubeStudioAdapter.Models;
using WebSocketSharp;
using WebSocket = WebSocketSharp.WebSocket;

namespace VtubeStudioAdapter.Services
{
    public class PhysicsService
    {
        private readonly WebSocket _client;
        private readonly ILogger _logger;
        private Action<VTSData>? _globalAction;

        public PhysicsService(WebSocket client, ILogger<PhysicsService> logger)
        {
            _client = client;
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

            _client.Send(buffer);
            _logger.LogInformation("Exiting {Method}", nameof(ChangePhysicParametr));
        }

        public async Task<VTSData> GetPhysicsParametrs(GetPhysicsQuery query)
        {
            _client.OnMessage += OnCompleted;

            _logger.LogInformation("Entering {Method}", nameof(GetPhysicsParametrs));
            const string Request = "GetCurrentModelPhysicsRequest";
            await SendRequest(Request, new VTSData());
            _globalAction = query.OnCompleted;

            _logger.LogInformation("Exiting {Method}", nameof(GetPhysicsParametrs));
            return new VTSData();
        }

        private async Task SendRequest(string messageType, VTSData data)
        {
            _logger.LogDebug("Sending physics request {MessageType}", messageType);
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
                    _logger.LogError($"[{DateTime.UtcNow}]: data in physics response was null");
                    return;
                }

                if (obj.Data.ErrorID is not null)
                {
                    _logger.LogWarning($"[{DateTime.UtcNow}]: VTube Studio API physics error: {obj.Data.ErrorID} {obj.Data.Message}");
                }

                _globalAction?.Invoke(obj.Data);
                _client.OnMessage -= OnCompleted;
                _globalAction = null;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing physics response");
            }
        }
    }
}