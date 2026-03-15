using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VtubeStudioAdapter.Models
{
    public class VTSData
    {
        [JsonProperty("pluginName")]
        public string? PluginName { get; set; }

        [JsonProperty("pluginDeveloper")]
        public string? PluginDeveloper { get; set; }
        [JsonProperty("pluginIcon")]
        public string? PluginIcon { get; set; }
        [JsonProperty("authenticationToken")]
        public string? AuthToken { get; set; }
        [JsonProperty("active")]
        public bool? Active { get; set; }

        [JsonProperty("vTubeStudioVersion")]
        public string VTubeStudioVersion { get; set; }

        [JsonProperty("currentSessionAuthenticated")]
        public bool CurrentSessionAuthenticated { get; set; }
        public PhysicParamter[]? Strength { get; set; }
        [JsonProperty("windOverrides")]
        public PhysicParamter[]? Wind { get; set; }
        [JsonProperty("errorID")]
        public int? ErrorID { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }
        public class PhysicParamter
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("value")]
            public double? Value { get; set; }

            [JsonProperty("setBaseValue")]
            public bool? SetBaseValue { get; set; }

            [JsonProperty("overrideSeconds")]
            public int? OverrideSeconds { get; set; }
        }

        [JsonProperty("modelHasPhysics")]
        public bool? ModelHasPhysics { get; set; }

        [JsonProperty("physicsSwitchedOn")]
        public bool? PhysicsSwitchedOn { get; set; }

        [JsonProperty("usingLegacyPhysics")]
        public bool? UsingLegacyPhysics { get; set; }

        [JsonProperty("physicsFPSSetting")]
        public int? PhysicsFPSSetting { get; set; }

        [JsonProperty("baseStrength")]
        public int? BaseStrength { get; set; }

        [JsonProperty("baseWind")]
        public int? BaseWind { get; set; }

        [JsonProperty("apiPhysicsOverrideActive")]
        public bool? ApiPhysicsOverrideActive { get; set; }

        [JsonProperty("apiPhysicsOverridePluginName")]
        public string? ApiPhysicsOverridePluginName { get; set; }
        [JsonProperty("physicsGroups")]
        public List<PhysicsGroup>? PhysicsGroups { get; set; }
        public class PhysicsGroup
        {
            [JsonProperty("groupID")]
            public string? GroupID { get; set; }

            [JsonProperty("groupName")]
            public string? GroupName { get; set; }

            [JsonProperty("strengthMultiplier")]
            public double? StrengthMultiplier { get; set; }

            [JsonProperty("windMultiplier")]
            public double? WindMultiplier { get; set; }
        }
        [JsonProperty("modelLoaded")]
        public bool? ModelLoaded { get; set; }

        [JsonProperty("numberOfArtMeshNames")]
        public int? NumberOfArtMeshNames { get; set; }

        [JsonProperty("numberOfArtMeshTags")]
        public int? NumberOfArtMeshTags { get; set; }

        [JsonProperty("artMeshNames")]
        public string[]? ArtMeshNames { get; set; }

        [JsonProperty("artMeshTags")]
        public string[]? ArtMeshTags { get; set; }
        [JsonProperty("customParameters")]
        public List<ArtMeshParameter>? CustomParametrs { get; set; }
        [JsonProperty("defaultParameters")]
        public List<ArtMeshParameter>? DefaultParametrs { get; set; }
        public class ArtMeshParameter
        {
            [JsonProperty("name")]
            public string? Name { get; set; }

            [JsonProperty("addedBy")]
            public string? AddedBy { get; set; }

            [JsonProperty("value")]
            public double? Value { get; set; }

            [JsonProperty("min")]
            public string? Min { get; set; }

            [JsonProperty("max")]
            public string? Max { get; set; }

            [JsonProperty("defaultValue")]
            public string? DefaultValue { get; set; }
        }
        [JsonProperty("parameterValues")]
        public Parametr[]? ParameterValues;
        [JsonProperty("faceFound")]
        public bool? FaceFound { get; set; }

        [JsonProperty("mode")]
        public string? Mode { get; set; }
        public class Parametr
        {
            [JsonProperty("id")]
            public string? Id { get; set; }

            [JsonProperty("weight")]
            public double? Weight { get; set; }

            [JsonProperty("value")]
            public double? Value { get; set; }
        }
        [JsonProperty("vtsModelName")]
        public string? VtsModelName { get; set; }

        [JsonProperty("vtsModelIconName")]
        public string? VtsModelIconName { get; set; }

        [JsonProperty("live2DModelName")]
        public string? Live2DModelName { get; set; }

        [JsonProperty("modelLoadTime")]
        public int? ModelLoadTime { get; set; }

        [JsonProperty("timeSinceModelLoaded")]
        public int? TimeSinceModelLoaded { get; set; }

        [JsonProperty("numberOfLive2DParameters")]
        public int? NumberOfLive2DParameters { get; set; }

        [JsonProperty("numberOfLive2DArtmeshes")]
        public int? NumberOfLive2DArtmeshes { get; set; }

        [JsonProperty("hasPhysicsFile")]
        public bool? HasPhysicsFile { get; set; }

        [JsonProperty("numberOfTextures")]
        public int? NumberOfTextures { get; set; }

        [JsonProperty("textureResolution")]
        public int? TextureResolution { get; set; }
        public class ModelPosition
        {
            [JsonProperty("positionX")]
            public double PositionX { get; set; }

            [JsonProperty("positionY")]
            public double PositionY { get; set; }

            [JsonProperty("rotation")]
            public double Rotation { get; set; }

            [JsonProperty("size")]
            public double Size { get; set; }

        }
        [JsonProperty("positionX")]
        public double? PositionX { get; set; }

        [JsonProperty("positionY")]
        public double? PositionY { get; set; }

        [JsonProperty("rotation")]
        public double? Rotation { get; set; }

        [JsonProperty("size")]
        public double? Size { get; set; }
        [JsonProperty("timeInSeconds")]
        public double? TimeInSeconds { get; set; }

        [JsonProperty("valuesAreRelativeToModel")]
        public bool? ValuesAreRelativeToModel { get; set; }
        [JsonProperty("modelPosition")]
        public ModelPosition? Position { get; set; }
        [JsonProperty("modelName")]
        public string? ModelName { get; set; }

        [JsonProperty("modelID")]
        public string? ModelID { get; set; }

    }

}
