using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VtubeStudioAdapter.Commands.PropertyModel.Movement;
using VtubeStudioAdapter.Models;
using WebSocketSharp;
using WebSocket = WebSocketSharp.WebSocket;

namespace VtubeStudioAdapter.Services
{
    public class MovementService
    {
        private readonly WebSocket _client;
        private readonly ILogger _logger;
        private Action<VTSData>? _globalAction;

        public MovementService(WebSocket client, ILogger<MovementService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<VTSData> GetArtMeshes(ArtMeshModelQuery query)
        {
            _client.OnMessage += OnCompleted;

            _logger.LogInformation("Entering {Method}", nameof(GetArtMeshes));
            const string Request = "ArtMeshListRequest";

            await SendRequest(Request, new VTSData());
            _globalAction = query.OnCompleted;

            _logger.LogInformation("Exiting {Method}", nameof(GetArtMeshes));
            return new VTSData();

        }

        public async Task<VTSData> GetTrackingParametrs(TrackingParametrsQuery query)
        {
            _client.OnMessage += OnCompleted;

            _logger.LogInformation("Entering {Method}", nameof(GetTrackingParametrs));
            const string Request = "InputParameterListRequest";

            await SendRequest(Request, new VTSData());
            _globalAction = query.OnCompleted;

            _logger.LogInformation("Exiting {Method}", nameof(GetTrackingParametrs));
            return new VTSData();

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

            _client.Send(buffer);
            _logger.LogInformation("Exiting {Method}", nameof(ChangeValueParametrs));
        }

        private async Task SendRequest(string messageType, VTSData data)
        {
            _logger.LogInformation("Sending movement request {MessageType}", messageType);
            var model = VtubeStudioModel.CreateModel(ConstStorage.API_NAME, ConstStorage.VERSION, messageType, data);

            var json = JsonConvert.SerializeObject(model, ConstStorage.SETTINGS);

            _logger.LogInformation("Json on movement method {0}", json);

            var buffer = Encoding.UTF8.GetBytes(json);

            _client.Send(buffer);
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

                _globalAction(obj.Data);
                _client.OnMessage -= OnCompleted;
                _globalAction = null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing movement response");
            }
        }
    }
}
