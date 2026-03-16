using MediatR;
using Newtonsoft.Json;
using AutoMapper;
using VtubeStudioAdapter.Models;
using static VtubeStudioAdapter.Models.VTSData;
namespace VtubeStudioAdapter.Сommands.Position;

public class GetCurrentModelQuery : IRequest<VTSData>
{
    public string? PluginName { get; set; }

    public string? VtsModelName { get; set; }

    public string? VtsModelIconName { get; set; }

    public string? Live2DModelName { get; set; }

    public int ModelLoadTime { get; set; }

    public int TimeSinceModelLoaded { get; set; }

    public int NumberOfLive2DParameters { get; set; }

    public int NumberOfLive2DArtmeshes { get; set; }

    public bool HasPhysicsFile { get; set; }

    public int NumberOfTextures { get; set; }

    public int TextureResolution { get; set; }
    public bool ModelLoaded { get; set; }

    public string? ModelName { get; set; }

    public string? ModelID { get; set; }

    public ModelPosition? ModelPosition { get; set; }

    public required Action<VTSData> OnCompleted { get; set; }

    public static implicit operator VTSData(GetCurrentModelQuery model)
    {
        var map = new Mapper(new MapperConfiguration(cfg =>
            cfg.CreateMap<GetCurrentModelQuery, VTSData>()
                .ForMember((data => data.PluginName), opt => opt.MapFrom(x => x.PluginName))
                .ForMember((data => data.VtsModelName), opt => opt.MapFrom(x => x.VtsModelName))
                .ForMember((data => data.VtsModelIconName), opt => opt.MapFrom(x => x.VtsModelIconName))
                .ForMember((data => data.Live2DModelName), opt => opt.MapFrom(x => x.Live2DModelName))
                .ForMember((data => data.ModelLoadTime), opt => opt.MapFrom(x => x.ModelLoadTime))
                .ForMember((data => data.TimeSinceModelLoaded), opt => opt.MapFrom(x => x.TimeSinceModelLoaded))
                .ForMember((data => data.NumberOfLive2DParameters), opt => opt.MapFrom(x => x.NumberOfLive2DParameters))
                .ForMember((data => data.NumberOfLive2DArtmeshes), opt => opt.MapFrom(x => x.NumberOfLive2DArtmeshes))
                .ForMember((data => data.HasPhysicsFile), opt => opt.MapFrom(x => x.HasPhysicsFile))
                .ForMember((data => data.NumberOfTextures), opt => opt.MapFrom(x => x.NumberOfTextures))
                .ForMember((data => data.TextureResolution), opt => opt.MapFrom(x => x.TextureResolution))
                .ForMember((data => data.ModelLoaded), opt => opt.MapFrom(x => x.ModelLoaded))
                .ForMember((data => data.ModelName), opt => opt.MapFrom(x => x.ModelName))
                .ForMember((data => data.ModelID), opt => opt.MapFrom(x => x.ModelID))
                .ForMember((data => data.Position), opt => opt.MapFrom(x => x.ModelPosition))
        ));
        var data = map.Map<GetCurrentModelQuery, VTSData>(model);
        return data;
    }

}
