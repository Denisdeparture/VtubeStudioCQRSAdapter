using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Newtonsoft.Json;
using VtubeStudioAdapter.Models;

namespace VtubeStudioAdapter.Commands.Auth
{
    public class AuthQuery : IRequest<VTSData>
    {
        public string? PluginName { get; set; }

        public string? PluginDeveloper { get; set; }
        public string? PluginIcon { get; set; }
        public string? AuthToken { get; set; }

        public static implicit operator VTSData(AuthQuery model)
        {
            var map = new Mapper(new MapperConfiguration(cfg =>
           cfg.CreateMap<AuthQuery, VTSData>()
           .ForMember((data => data.PluginName), opt => opt.MapFrom(x => x.PluginName))
           .ForMember((data => data.PluginIcon), opt => opt.MapFrom(x => x.PluginIcon))
           .ForMember((data => data.PluginDeveloper), opt => opt.MapFrom(x => x.PluginDeveloper))
           .ForMember((data => data.AuthToken), opt => opt.MapFrom(x => x.AuthToken))
           ));
            var data = map.Map<AuthQuery, VTSData>(model);
            return data;

        }

    }
}