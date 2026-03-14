using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VtubeStudioAdapter.Models
{
    public partial class VtubeStudioModel : VtubeBaseModelV1
    {
        [JsonProperty("data")]
        public VTSData? Data { get; set; }
    }
    public partial class VtubeStudioModel
    {
        public static VtubeStudioModel CreateModel(string apiName, string version, string request, VTSData data)
        {
            return new VtubeStudioModel()
            {
                ApiName = apiName,
                ApiVersion = version,
                MessageType = request,
                Data = data
            };
        }
    }
}