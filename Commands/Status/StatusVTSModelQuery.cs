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
        public bool Active { get; set; }

        public int Port { get; set; }

        public string? InstanceID { get; set; }

        public string? WindowTitle { get; set; }

        public static implicit operator VTSData(StatusVTSModelQuery model)
        {
            var map = new Mapper(new MapperConfiguration(cfg =>
                cfg.CreateMap<StatusVTSModelQuery, VTSData>()
                    .ForMember((data => data.Active), opt => opt.MapFrom(x => x.Active))
                    .ForMember((data => data.Port), opt => opt.MapFrom(x => x.Port))
                    .ForMember((data => data.InstanceID), opt => opt.MapFrom(x => x.InstanceID))
                    .ForMember((data => data.WindowTitle), opt => opt.MapFrom(x => x.WindowTitle))
            ));
            var data = map.Map<StatusVTSModelQuery, VTSData>(model);
            return data;
        }
    }

}