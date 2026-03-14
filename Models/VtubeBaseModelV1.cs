using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VtubeStudioAdapter.Models
{
    public class VtubeBaseModelV1
    {
        [JsonProperty("apiName")]
        public string ApiName { get; set; } = null!;

        [JsonProperty("apiVersion")]
        public string ApiVersion { get; set; } = null!;

        [JsonProperty("messageType")]
        public string MessageType { get; set; } = null!;
    }

}