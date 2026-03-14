using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VtubeStudioAdapter.Models;

namespace VtubeStudioAdapter
{
    public class AuthService
    {

        private ClientWebSocket _client;

        private CancellationTokenSource _cts;

        private ILogger _logger;


        public AuthService(ClientWebSocket socket, CancellationTokenSource cts, ILogger<AuthService> logger)
        {
            _logger = logger;
            _cts = cts;
            _client = socket;

            _client.ConnectAsync(new Uri(ConstStorage.vtube_studio_uri), _cts.Token);
        }
        public async Task<VTSData> AuthApp(VTSData data)
        {
            _logger.LogInformation("Entering {Method}", nameof(AuthApp));
            const string Request = "AuthenticationTokenRequest";

            data.PluginIcon = Base64(data.PluginIcon);

            var model = VtubeStudioModel.CreateModel(ConstStorage.API_NAME, ConstStorage.VERSION, Request, data);

            var json = JsonConvert.SerializeObject(model, ConstStorage.SETTINGS);

            var buffer = Encoding.UTF8.GetBytes(json);

            await _client.SendAsync(
             new ArraySegment<byte>(buffer),
             WebSocketMessageType.Text,
             true,
             _cts.Token
            );
            _logger.LogInformation("Exiting {Method}", nameof(AuthApp));
            return await Auth();
        }
        private async Task<VTSData> Auth()
        {
            _logger.LogInformation("Entering {Method}", nameof(Auth));
            var buffer = new byte[1024];
            WebSocketReceiveResult? result = null;
            VTSData? data = null;
            do
            {
                result = await _client.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None
                );
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    if (result.EndOfMessage)
                    {
                        string json = Encoding.UTF8.GetString(buffer, 0, result.Count);

                        var obj = JsonConvert.DeserializeObject<VtubeStudioModel>(json) ?? throw new NullReferenceException("Recive obj was null");

                        if (obj.Data is null)
                        {
                            _logger.LogError($"[{DateTime.UtcNow}]: data in {obj} was null");

                            await _client.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Закрыто из-за ошибки",
                            _cts.Token
                            );

                            _cts.Cancel();

                            return null;
                        }

                        var token = obj.Data.AuthToken;

                        if (obj.Data.ErrorID is not null)
                        {
                            _logger.LogWarning($"[{DateTime.UtcNow}]: Vtube studio return error: {obj.Data.ErrorID} {obj.Data.Message} ");
                        }
                        data = obj.Data;
                    }
                }
            } while (result is null && !result.CloseStatus.HasValue);
            _logger.LogInformation("Exiting {Method}", nameof(Auth));
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

            await _client.SendAsync(
             new ArraySegment<byte>(buffer),
             WebSocketMessageType.Text,
             true,
             _cts.Token
            );
            await Auth();
            _logger.LogInformation("Exiting {Method}", nameof(AuthWithToken));
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