using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using VtubeStudioAdapter.Models;

namespace VtubeStudioAdapter.Commands
{
    public class StatusVTSModelQuery : IRequest<VTSData>
    {
        public required string? PluginName { get; set; }

        public bool Active { get; set; }

        public string? VTubeStudioVersion { get; set; }

        public bool CurrentSessionAuthenticated { get; set; }

        public required Action<VTSData> OnCompleted { get; set; }

        public static implicit operator VTSData(StatusVTSModelQuery model)
        {
            var map = new Mapper(new MapperConfiguration(cfg =>
                cfg.CreateMap<StatusVTSModelQuery, VTSData>()
                    .ForMember((data => data.PluginName), opt => opt.MapFrom(x => x.PluginName))
                    .ForMember((data => data.Active), opt => opt.MapFrom(x => x.Active))
                    .ForMember((data => data.CurrentSessionAuthenticated), opt => opt.MapFrom(x => x.CurrentSessionAuthenticated))
                    .ForMember((data => data.VTubeStudioVersion), opt => opt.MapFrom(x => x.VTubeStudioVersion))
            ));
            var data = map.Map<StatusVTSModelQuery, VTSData>(model);
            return data;
        }
    }

}