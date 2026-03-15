using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VtubeStudioAdapter.Models
{
    public class Plugin
    {

        public string? PluginName { get; set; }

        public string? PluginDeveloper { get; set; }

        public string? PathToIcon { get; set; }
    }
}