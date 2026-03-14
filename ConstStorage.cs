using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VtubeStudioAdapter
{
    public static class ConstStorage
    {
        public const string vtube_studio_uri = "ws://localhost:8001";
        public const string VERSION = "1.0";
        public const string API_NAME = "VTubeStudioPublicAPI";
        public static JsonSerializerSettings SETTINGS = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }
}