using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VtubeStudioAdapter.Commands.Auth;
using VtubeStudioAdapter.Models;
using WebSocketSharp;

using WebSocket = WebSocketSharp.WebSocket;
namespace VtubeStudioAdapter
{
    public class AuthService
    {

        private WebSocket _client;

        private ILogger _logger;

        private Action<VTSData>? _globalAction;

        public AuthService(WebSocket socket, ILogger<AuthService> logger)
        {
            _logger = logger;
            _client = socket;
            _client.Connect();
            _client.OnMessage += OnComleted;
        }
        public async Task<VTSData> AuthApp(AuthQuery query)
        {
            var data = (VTSData)query;
            _logger.LogInformation("Entering {Method}", nameof(AuthApp));
            const string Request = "AuthenticationTokenRequest";

            data.PluginIcon = Base64(data.PluginIcon);

            var model = VtubeStudioModel.CreateModel(ConstStorage.API_NAME, ConstStorage.VERSION, Request, data);

            var json = JsonConvert.SerializeObject(model, ConstStorage.SETTINGS);

            var buffer = Encoding.UTF8.GetBytes(json);

            _client.Send(buffer);


            _globalAction = query.OnCompleted!;

            _logger.LogInformation("Exiting {Method}", nameof(AuthApp));

            return data;
        }
        public async void AuthWithToken(string token, string pluginName, string pluginDeveloper)
        {
            _logger.LogInformation("Entering {Method}", nameof(AuthWithToken));
            const string Request = "AuthenticationRequest";

            var data = new VTSData()
            {
                AuthToken = token,
                PluginDeveloper = pluginDeveloper,
                PluginName = pluginName
            };

            var model = VtubeStudioModel.CreateModel(ConstStorage.API_NAME, ConstStorage.VERSION, Request, data);

            var json = JsonConvert.SerializeObject(model, ConstStorage.SETTINGS);

            var buffer = Encoding.UTF8.GetBytes(json);

            _client.Send(buffer);

            _logger.LogInformation("Exiting {Method}", nameof(AuthWithToken));

        }

        private async void OnComleted(object? sender, MessageEventArgs e)
        {
            var data = await WorkWithJson(e.Data);

            if (data is null)
            {
                _logger.LogWarning($"[{DateTime.UtcNow}]: data was null");
            }

            _globalAction(data!);

            _client.OnMessage -= OnComleted;
            _globalAction = null;

        }

        private async Task<VTSData?> WorkWithJson(string json)
        {
            var obj = JsonConvert.DeserializeObject<VtubeStudioModel>(json) ?? throw new NullReferenceException("Recive obj was null");

            if (obj.Data is null)
            {
                _logger.LogError($"[{DateTime.UtcNow}]: data in {obj} was null");
                return null;
            }

            var token = obj.Data.AuthToken;

            if (obj.Data.ErrorID is not null)
            {
                _logger.LogWarning($"[{DateTime.UtcNow}]: Vtube studio return error: {obj.Data.ErrorID} {obj.Data.Message} ");
            }
            return obj.Data;
        }
        private string Base64(string? path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return "";
            }
            using (var file = File.Open(path, FileMode.Open))
            {
                byte[] buffer = new byte[file.Length];

                if (!file.CanRead)
                {
                    _logger.LogError("File {0} impossible for ridding", path);
                }

                file.Read(buffer, 0, (int)file.Length);

                string base64 = Convert.ToBase64String(buffer);

                return base64;
            }
        }

    }
}